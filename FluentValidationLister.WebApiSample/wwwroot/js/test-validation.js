$(function () {
    var personForm = $("#personForm"); // Form
    var resultPanel = $("#resultPanel"); // Div

    // AJAX settings
    $.ajaxSetup({
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {},
        cache: false
    });

    // AJAX error handler
    $(document).ajaxError(function (e, xhr) {
        if (xhr.status > 400) {
            if (console.error) console.error(xhr.responseText);

            var container = $("<pre />").text(xhr.responseText).addClass("error");
            resultPanel.append($("<p />").append(container));
        }
    });

    // Get the validation meta-data for the Person form
    var validationList;
    $.post("api/Person?validation=true", "{}", data => validationList = data);

    // Validate form fields against validation meta-data
    var validateForm = function () {
        var data = getFormValues();
        var errorList = [];

        $.each(data, function (fieldName, fieldValue) {
            var fieldIsValid = true;
            for (var ruleName in validationList.validatorList[fieldName]) {
                var fieldPassesRule = true;
                var fieldHasValue = fieldValue !== null && fieldValue !== "";
                switch (ruleName) {
                    case "required":
                        fieldPassesRule = fieldHasValue;
                        break;
                    case "regex":
                        if (fieldHasValue === true) {
                            var regex = new RegExp(validationList.validatorList[fieldName][ruleName], "g");
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
                        if (fieldHasValue === true) fieldPassesRule = fieldValue.length >= parseInt(validationList.validatorList[fieldName][ruleName]);
                        break;
                    case "maxLength":
                        if (fieldHasValue === true) fieldPassesRule = fieldValue.length <= parseInt(validationList.validatorList[fieldName][ruleName]);
                        break;
                    case "exactLength":
                        if (fieldHasValue === true) fieldPassesRule = fieldValue.length === parseInt(validationList.validatorList[fieldName][ruleName]);
                        break;
                    case "length":
                        if (fieldHasValue === true) {
                            fieldPassesRule =
                                fieldValue.length >= validationList.validatorList[fieldName][ruleName].min &&
                                fieldValue.length <= validationList.validatorList[fieldName][ruleName].max;
                        }
                        break;
                    case "range":
                        if (fieldHasValue === true) {
                            fieldPassesRule =
                                fieldValue >= validationList.validatorList[fieldName][ruleName].from &&
                                fieldValue <= validationList.validatorList[fieldName][ruleName].to;
                        }
                        break;
                    case "exclusiveRange":
                        if (fieldHasValue === true) {
                            fieldPassesRule =
                                fieldValue < validationList.validatorList[fieldName][ruleName].from &&
                                fieldValue > validationList.validatorList[fieldName][ruleName].to;
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
            if (console.error) console.error("Clientside validation failed!");

            var list = $("<ul />").addClass("error");
            $.each(errorList, (i, val) => list.append($("<li />").text(val)));

            resultPanel.append(list);
        }

        return errorList.length === 0;
    };

    // Format the form values into an object
    var getFormValues = function (splitField) {
        var data = {};
        personForm.serializeArray().map(function (field) {
            if (splitField === true && field.name.includes(".")) {
                var split = field.name.split(".");
                if (!data[split[0]]) data[split[0]] = {};

                data[split[0]][split[1]] = field.value;

            } else {
                data[field.name] = field.value;
            }
        });

        // Correct any string-types that should be numeric or Boolean (only one in this example)
        if (data.age === "") data.age = null; else data.age = parseInt(data.age);

        return data;
    };

    // Clear any previous error/success result
    var resetResult = function () {
        $(".error").removeClass("error");
        resultPanel.empty();
    };

    // Populate form fields with data from the server
    var populateFormFromJson = function (json, prefix = "") {
        $.each(json, function (key, val) {
            if (typeof val === "object") populateFormFromJson(val, key + ".");
            else $('[name="' + prefix + key + '"]', personForm).val(val);
        });
    };

    // Button click event
    $("#populateFormButton").click(function () {
        resetResult();

        $.getJSON("api/Person/1", (json) => populateFormFromJson(json));
    });

    // Form submission event
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
                .done(data => resultPanel.append($("<p />").text(data.message)))
                .fail(function (data) {
                    if (data.status === 400) {
                        // Validation failed - display the errors
                        if (console.error) console.error("Server-side validation failed!");

                        var validationResponse = data.responseJSON;
                        var errorList = [];

                        for (var key in validationResponse.errors) {
                            $('[name="' + key + '"]', personForm).addClass("error");
                            $.each(validationResponse.errors[key], (i, val) => errorList.push(val));
                        }

                        var list = $("<ul />").addClass("error");
                        $.each(errorList, (i, val) => list.append($("<li />").text(val)));

                        resultPanel.append(list);
                    }
                });
        });
});
