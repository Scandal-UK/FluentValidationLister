$(function () {
    $.ajaxSetup({
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {},
        cache: false
    });

    var validationList;
    $.post("api/Person?validation=true", "{}", function (data) {
        validationList = data;
    });

    var personForm = $("#personForm"); // Form
    var resultPanel = $("#resultPanel"); // Div

    var validateForm = function () {
        var data = getFormValues();
        var errorList = [];

        $.each(data, function (fieldName, fieldValue) {
            var valid = true;
            for (var ruleName in validationList.validatorList[fieldName]) {
                switch (ruleName) {
                    case "required":
                        if (!fieldValue || fieldValue === "") {
                            valid = false;
                            errorList.push(validationList.errorList[fieldName][ruleName]);
                        }
                        break;
                    // todo: default to AJAX validation for any unrecognised ruleName
                }
            }

            if (valid === false) $('[name="' + fieldName + '"]', personForm).addClass("error");
        });

        if (errorList.length > 0) {
            if (console.error) console.error("Clientside validation failed!");

            var list = $("<ul />").addClass("error");
            $.each(errorList, function (i, val) {
                list.append($("<li />").text(val));
            });

            resultPanel.append(list);
        }

        return errorList.length === 0;
    };

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

    $("#personForm").submit(function (e) {
        e.preventDefault();

        if (personForm.prop("disabled") === true) return;
        resetResult();

        if ($("#performClientsideValidation").is(":checked")) {
            if (validateForm() === false) return;
        }

        personForm.prop("disabled", true);

        $.post("api/Person", JSON.stringify(getFormValues(true)))
            .always(function () {
                personForm.prop("disabled", false);
            })
            .done(function (data) {
                resultPanel.append($("<p />").text(data.message));
            })
            .fail(function (data) {
                if (console.error) console.error("Server-side validation failed!");

                var validationResponse = data.responseJSON;
                var errorList = [];

                for (var key in validationResponse.errors) {
                    $('[name="' + key + '"]', personForm).addClass("error");
                    errorList.push(validationResponse.errors[key]);
                }

                var list = $("<ul />").addClass("error");
                $.each(errorList, function (i, val) {
                    list.append($("<li />").text(val));
                });

                resultPanel.append(list);
            });
    });

    $("#populateFormButton").click(function () {
        resetResult();

        $.getJSON("api/Person/1", function (json) {
            $.each(json, function (key, val) {
                if (key === "address") {
                    $.each(val, function (addrKey, addrVal) {
                        $('[name="address.' + addrKey + '"]', personForm).val(addrVal);
                    });
                } else {
                    $('[name="' + key + '"]', personForm).val(val);
                }
            });
        });
    });

    var resetResult = function () {
        $(".error").removeClass("error");
        resultPanel.empty();
    };
});
