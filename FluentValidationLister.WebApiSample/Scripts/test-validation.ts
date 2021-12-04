// Simple definition of a range
interface RangeValues {
    from: number;
    to: number;
}

// Simple definition of a limited range
interface LimitValues {
    min: number;
    max: number;
}

// Simple definition of a generic dictionary using string-based keys
interface Dictionary<T> {
    [key: string]: T;
}

// TypeScript definition of the validation meta-data payload
interface ValidatorRules {
    validatorList: Dictionary<Dictionary<string | boolean | number | RangeValues | LimitValues>>;
    errorList: Dictionary<Dictionary<string>>;
    typeList: Dictionary<string>;
}

// Simple jQuery application - document-ready event
$(function () {
    const personForm = $("#personForm"); // Defines a reference for the form element
    const resultPanel = $("#resultPanel"); // Defines a reference for the output div element
    let validationList: ValidatorRules; // Defines a reference for the validation meta-data

    // Set the default settings for all XHR requests
    $.ajaxSetup({
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {},
        cache: false
    });

    // Add a DEBUG handler for unknown errors returned from the server (>400)
    $(document).ajaxError(function (_, xhr: JQueryXHR) {
        if (xhr.status > 400) {
            if (console.error) console.error(xhr.responseText);

            const container = $("<pre />").text(xhr.responseText).addClass("error");
            resultPanel.append($("<p />").append(container));
        }
    });

    // Define a method that formats a string value - based on the declared JSON type
    const getValue = function (fieldName: string, fieldValue: string) {
        switch (validationList.typeList[fieldName])
        {
            case "boolean": return fieldValue === "" ? null : fieldValue === "true";
            case "number": return fieldValue === "" ? null : Number(fieldValue);
            default: return fieldValue === "" ? null : fieldValue;
        }
    };

    // Define a method that formats the form values into an object
    const getFormValues = function (splitField?: boolean) {
        const data: Dictionary<string | number | boolean | {} | null> = {};
        personForm.serializeArray().map(function (field) {
            if (splitField === true && field.name.includes(".")) {
                const split = field.name.split(".");
                if (typeof data[split[0]] === "undefined") data[split[0]] = {};
                (data[split[0]] as Dictionary<string | number | boolean | null>)[split[1]] = getValue(field.name, field.value);

            } else {
                data[field.name] = getValue(field.name, field.value);
            }
        });

        // To include false values for checkboxes, we must add them separately from jQuery serializeArray()
        $("input:checkbox:not(:checked)", personForm).each(function () {
            const field = this as HTMLInputElement;
            data[field.name] = false;
        });

        return data;
    };

    // Define a method that clears any previous error/success result
    const resetResult = function () {
        $(".error").removeClass("error");
        resultPanel.empty();
    };

    // Define a method that populates form fields with data returned from the server
    const populateFormFromJson = function ({ json, prefix = "" }: { json: unknown; prefix?: string }) {
        $.each(json, function (key: string, val) {
            if (typeof val === "object") populateFormFromJson({ json: val, prefix: key + "." });
            else {
                const formField = $('[name="' + prefix + key + '"]', personForm);
                if (formField.is(":checkbox")) formField.prop("checked", val); else formField.val(val);
            }
        });
    };

    // Define a method that validates form fields against the validation meta-data
    const validateForm = function () {
        const data = getFormValues();
        const errorList: Array<string> = [];

        $.each(data, function (fieldName: string, fieldValue) {
            let fieldIsValid = true;
            for (const ruleName in validationList.validatorList[fieldName]) {
                const fieldHasValue = fieldValue !== null && fieldValue !== "";
                let fieldPassesRule = true;

                switch (ruleName) {
                    case "required":
                        if (validationList.validatorList[fieldName][ruleName] as boolean) fieldPassesRule = fieldHasValue;
                        break;
                    case "regex":
                        if (fieldHasValue === true) {
                            const regex = new RegExp(validationList.validatorList[fieldName][ruleName] as string, "g");
                            fieldPassesRule = regex.test(fieldValue as string);
                        }
                        break;
                    case "lessThan":
                        if (fieldHasValue === true) fieldPassesRule = fieldValue as number < validationList.validatorList[fieldName][ruleName];
                        break;
                    case "greaterThan":
                        if (fieldHasValue === true) fieldPassesRule = fieldValue as number > validationList.validatorList[fieldName][ruleName];
                        break;
                    case "lessThanOrEqualTo":
                        if (fieldHasValue === true) fieldPassesRule = fieldValue as number <= validationList.validatorList[fieldName][ruleName];
                        break;
                    case "greaterThanOrEqualTo":
                        if (fieldHasValue === true) fieldPassesRule = fieldValue as number >= validationList.validatorList[fieldName][ruleName];
                        break;
                    case "minLength":
                        if (fieldHasValue === true) fieldPassesRule = (fieldValue as string).length >= (validationList.validatorList[fieldName][ruleName] as number);
                        break;
                    case "maxLength":
                        console.log(validationList.validatorList[fieldName][ruleName]);
                        if (fieldHasValue === true) fieldPassesRule = (fieldValue as string).length <= (validationList.validatorList[fieldName][ruleName] as number);
                        break;
                    case "exactLength":
                        if (fieldHasValue === true) fieldPassesRule = (fieldValue as string).length === (validationList.validatorList[fieldName][ruleName] as number);
                        break;
                    case "length":
                        if (fieldHasValue === true) {
                            fieldPassesRule =
                                (fieldValue as string).length >= (validationList.validatorList[fieldName][ruleName] as LimitValues).min &&
                                (fieldValue as string).length <= (validationList.validatorList[fieldName][ruleName] as LimitValues).max;
                        }
                        break;
                    case "range":
                        if (fieldHasValue === true) {
                            fieldPassesRule =
                                fieldValue as number >= (validationList.validatorList[fieldName][ruleName] as RangeValues).from &&
                                fieldValue as number <= (validationList.validatorList[fieldName][ruleName] as RangeValues).to;
                        }
                        break;
                    case "exclusiveRange":
                        if (fieldHasValue === true) {
                            fieldPassesRule =
                                fieldValue as number < (validationList.validatorList[fieldName][ruleName] as RangeValues).from &&
                                fieldValue as number > (validationList.validatorList[fieldName][ruleName] as RangeValues).to;
                        }
                        break;
                }

                // Compile the error message(s)
                if (fieldPassesRule === false) {
                    fieldIsValid = false;
                    errorList.push(validationList.errorList[fieldName][ruleName]);
                }
            }

            // Highlight the affected field
            if (fieldIsValid === false) $('[name="' + fieldName + '"]', personForm).addClass("error");
        });

        if (errorList.length > 0) {
            resultPanel.append($("<p />").addClass("errortitle").text("Clientside validation failed"));

            // Add the messages to the displayed result
            const list = $("<ul />").addClass("error");
            $.each(errorList, (_i, val) => list.append($("<li />").text(val)));

            resultPanel.append(list);
        }

        return errorList.length === 0;
    };

    // Add the button-click event that populates the form
    $("#populateFormButton").click(function () {
        resetResult();
        $.getJSON("api/Person/1", (json) => populateFormFromJson({ json }));
    });

    // Add the form submission event
    $("#personForm")
        .on("reset", resetResult)
        .on("submit", function (e) {
            e.preventDefault();

            // If form is already pending submission finish here
            if (personForm.prop("disabled") === true) return;

            // Clear any previous results from the UI
            resetResult();

            // If client-side validation fails, bail-out here (without posting the payload)
            if ($("#performClientsideValidation").is(":checked")) {
                if (validateForm() === false) return;
            }

            // Try to prevent subsequent submits(!)
            personForm.prop("disabled", true);

            // Post the form payload to the server
            $.post("api/Person", JSON.stringify(getFormValues(true)))
                .always(() => personForm.prop("disabled", false))
                .done(data => resultPanel.append($("<p />").addClass("bold").text(data.message)))
                .fail(function (data) {
                    if (data.status === 400) {
                        // Server-side validation failed
                        resultPanel.append($("<p />").addClass("errortitle").text("Server-side validation failed"));

                        const validationResponse = data.responseJSON;
                        const errorList: string[] = [];

                        // Highlight the affected fields and compile the error messages into an array
                        for (const key in validationResponse.errors) {
                            $('[name="' + key + '"]', personForm).addClass("error");
                            $.each(validationResponse.errors[key], (_i, val) => errorList.push(val));
                        }

                        // Add the array of error messages to the UI as a bullet list
                        const list = $("<ul />").addClass("error");
                        $.each(errorList, (_i, val) => list.append($("<li />").text(val)));

                        resultPanel.append(list);
                    }
                });
        });

    // Get the validation meta-data for the Person form
    $.post("api/Person?validation=true", "{}", data => validationList = data);
});
