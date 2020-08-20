interface Dictionary<T> {
    [key: string]: T;
}

interface ValidatorRules {
    validatorList: Dictionary<Dictionary<unknown>>;
    errorList: Dictionary<Dictionary<string>>;
    typeList: Dictionary<string>;
}

interface RangeValues {
    from: number;
    to: number;
}

interface LimitValues {
    min: number;
    max: number;
}

$(function () {
    const personForm = $("#personForm"); // Form
    const resultPanel = $("#resultPanel"); // Div

    // AJAX settings
    $.ajaxSetup({
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {},
        cache: false
    });

    // AJAX error handler
    $(document).ajaxError(function (_e, xhr) {
        if (xhr.status > 400) {
            if (console.error) console.error(xhr.responseText);

            const container = $("<pre />").text(xhr.responseText).addClass("error");
            resultPanel.append($("<p />").append(container));
        }
    });

    // Get the validation meta-data for the Person form
    let validationList: ValidatorRules;
    $.post("api/Person?validation=true", "{}", data => validationList = data);

    const getValue = function (fieldName: string, fieldValue: string) {
        switch (validationList.typeList[fieldName])
        {
            case "bool": return fieldValue === "" ? null : fieldValue === "true";
            case "number": return fieldValue === "" ? null : Number(fieldValue);
            default: return fieldValue;
        }
    };

    // Format the form values into an object
    const getFormValues = function (splitField?: boolean) {
        const data: Dictionary<unknown> = {};
        personForm.serializeArray().map(function (field) {
            if (splitField === true && field.name.includes(".")) {
                const split = field.name.split(".");
                if (typeof data[split[0]] === "undefined") data[split[0]] = {};
                (data[split[0]] as Dictionary<string>)[split[1]] = field.value;

            } else {
                data[field.name] = getValue(field.name, field.value);
            }
        });

        $("input:checkbox").each(function () {
            const field = this as HTMLInputElement;
            data[field.name] = field.checked;
        });

        return data;
    };

    // Clear any previous error/success result
    const resetResult = function () {
        $(".error").removeClass("error");
        resultPanel.empty();
    };

    // Populate form fields with data from the server
    const populateFormFromJson = function ({ json, prefix = "" }: { json: unknown; prefix?: string }) {
        $.each(json, function (key: string, val) {
            if (typeof val === "object") populateFormFromJson({ json: val, prefix: key + "." });
            else {
                const formField = $('[name="' + prefix + key + '"]', personForm);
                if (formField.is(":checkbox")) formField.prop("checked", val); else formField.val(val);
            }
        });
    };

    // Validate form fields against validation meta-data
    const validateForm = function () {
        const data = getFormValues();
        const errorList: Array<string> = [];

        $.each(data, function (fieldName, fieldValue: string) {
            let fieldIsValid = true;
            for (const ruleName in validationList.validatorList[fieldName]) {
                const fieldHasValue = fieldValue !== null && fieldValue !== "";
                let fieldPassesRule = true;

                switch (ruleName) {
                    case "required":
                        fieldPassesRule = fieldHasValue;
                        break;
                    case "regex":
                        if (fieldHasValue === true) {
                            const regex = new RegExp(validationList.validatorList[fieldName][ruleName] as string, "g");
                            fieldPassesRule = regex.test(fieldValue);
                        }
                        break;
                    case "lessThan":
                        if (fieldHasValue === true) fieldPassesRule = fieldValue < validationList.validatorList[fieldName][ruleName];
                        break;
                    case "greaterThan":
                        if (fieldHasValue === true) fieldPassesRule = fieldValue > validationList.validatorList[fieldName][ruleName];
                        break;
                    case "lessThanOrEqualTo":
                        if (fieldHasValue === true) fieldPassesRule = fieldValue <= validationList.validatorList[fieldName][ruleName];
                        break;
                    case "greaterThanOrEqualTo":
                        if (fieldHasValue === true) fieldPassesRule = fieldValue >= validationList.validatorList[fieldName][ruleName];
                        break;
                    case "minLength":
                        if (fieldHasValue === true) fieldPassesRule = (fieldValue as string).length >= parseInt(validationList.validatorList[fieldName][ruleName] as string);
                        break;
                    case "maxLength":
                        if (fieldHasValue === true) fieldPassesRule = (fieldValue as string).length <= parseInt(validationList.validatorList[fieldName][ruleName] as string);
                        break;
                    case "exactLength":
                        if (fieldHasValue === true) fieldPassesRule = (fieldValue as string).length === parseInt(validationList.validatorList[fieldName][ruleName] as string);
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
                                parseInt(fieldValue) >= (validationList.validatorList[fieldName][ruleName] as RangeValues).from &&
                                parseInt(fieldValue) <= (validationList.validatorList[fieldName][ruleName] as RangeValues).to;
                        }
                        break;
                    case "exclusiveRange":
                        if (fieldHasValue === true) {
                            fieldPassesRule =
                                parseInt(fieldValue) < (validationList.validatorList[fieldName][ruleName] as RangeValues).from &&
                                parseInt(fieldValue) > (validationList.validatorList[fieldName][ruleName] as RangeValues).to;
                        }
                        break;
                }

                if (fieldPassesRule === false) {
                    fieldIsValid = false;
                    errorList.push(validationList.errorList[fieldName][ruleName]);
                }
            }

            if (fieldIsValid === false) $('[name="' + fieldName + '"]', personForm).addClass("error");
        });

        if (errorList.length > 0) {
            resultPanel.append($("<p />").addClass("errortitle").text("Clientside validation failed"));

            const list = $("<ul />").addClass("error");
            $.each(errorList, (i, val) => list.append($("<li />").text(val)));

            resultPanel.append(list);
        }

        return errorList.length === 0;
    };

    // Button click event
    $("#populateFormButton").click(function () {
        resetResult();
        $.getJSON("api/Person/1", (json) => populateFormFromJson({ json }));
    });

    // Form events
    $("#personForm")
        .on("reset", resetResult)
        .on("submit", function (e) {
            e.preventDefault();

            if (personForm.prop("disabled") === true) return;
            resetResult();

            if ($("#performClientsideValidation").is(":checked")) {
                if (validateForm() === false) return;
            }

            personForm.prop("disabled", true);

            $.post("api/Person", JSON.stringify(getFormValues(true)))
                .always(() => personForm.prop("disabled", false))
                .done(data => resultPanel.append($("<p />").addClass("bold").text(data.message)))
                .fail(function (data) {
                    if (data.status === 400) {
                        // Server-side validation failed - display the errors
                        resultPanel.append($("<p />").addClass("errortitle").text("Server-side validation failed"));

                        const validationResponse = data.responseJSON;
                        const errorList: string[] = [];

                        for (const key in validationResponse.errors) {
                            $('[name="' + key + '"]', personForm).addClass("error");
                            $.each(validationResponse.errors[key], (_i, val) => errorList.push(val));
                        }

                        const list = $("<ul />").addClass("error");
                        $.each(errorList, (_i, val) => list.append($("<li />").text(val)));

                        resultPanel.append(list);
                    }
                });
        });
});
