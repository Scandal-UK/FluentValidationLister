(function (window, document, $) {
    // Checkbox, Button, Form, Div
    var performClientsideValidation, populateForm, personForm, resultPanel;

    // Response from FluentValidationLister
    var validationList;

    // todo: rewrite script for jQuery
    var validateForm = function () {
        var data = getFormValues();
        var form = $(personForm);
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

            if (valid === false) $('[name="' + fieldName + '"]', form).addClass("error");
        });

        if (errorList.length > 0)
        {
            if (console.error) console.error("Clientside validation failed!");

            var list = $("<ul />").addClass("error");
            $.each(errorList, function (i, val) {
                list.append($("<li />").text(val));
            });

            $(resultPanel).append(list);
        }

        return errorList.length === 0;
    };

    var submitForm = function (e) {
        e.preventDefault();

        if (personForm.disabled === true) return;
        resetResult();

        if ($(performClientsideValidation).is(":checked")) {
            if (validateForm() === false) return;
        }

        personForm.disabled = true;

        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = displayFormResult;
        xhr.open("POST", "api/Person", true);
        xhr.setRequestHeader("Content-Type", "application/json");

        xhr.send(JSON.stringify(getFormValues(true)));
    };

    var displayFormResult = function () {
        if (this.readyState === 4) {
            personForm.disabled = false;

            var validationResponse = JSON.parse(this.responseText);
            if (this.status === 400) {
                if (console.error) console.error("Server-side validation failed!");

                var form = $(personForm);
                var errorList = [];

                for (var key in validationResponse.errors) {
                    $('[name="' + key + '"]', form).addClass("error");
                    errorList.push(validationResponse.errors[key]);
                }

                var list = $("<ul />").addClass("error");
                $.each(errorList, function (i, val) {
                    list.append($("<li />").text(val));
                });

                $(resultPanel).append(list);

            } else {
                var result = $("<p />");
                if (this.status > 400) result.addClass("error");
                result.text(validationResponse.message);

                $(resultPanel).append(result);
            }
        }
    };

    var getFormValues = function (splitField) {
        var data = {};
        $(personForm).serializeArray().map(function (field) {
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

    var populateFormFromServer = function () {
        resetResult();

        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            if (this.readyState === 4) {
                var form = $(personForm);
                var json = JSON.parse(this.responseText);

                $.each(json, function (key, val) {
                    if (key === "address") {
                        $.each(val, function (addrKey, addrVal) {
                            $('[name="address.' + addrKey + '"]', form).val(addrVal);
                        });
                    } else {
                        $('[name="' + key + '"]', form).val(val);
                    }
                });
            }
        };

        xhr.open("GET", "api/Person/1", true);
        xhr.setRequestHeader("Accepts", "application/json");
        xhr.send();
    };

    var resetResult = function () {
        $(".error").removeClass("error");
        if (resultPanel.hasChildNodes()) resultPanel.removeChild(resultPanel.childNodes[0]);
    };

    var getValidationRules = function () {
        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            if (this.readyState === 4) validationList = JSON.parse(this.responseText);
        };

        xhr.open("POST", "api/Person?validation=true", true);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.send(JSON.stringify({ person: 1 }));
    };

    window.addEventListener("load", function () {
        getValidationRules();

        performClientsideValidation = document.getElementById("performClientsideValidation"); // Checkbox
        populateForm = document.getElementById("populateFormButton"); // Button
        personForm = document.getElementById("personForm"); // Form
        resultPanel = document.getElementById("resultPanel"); // Div

        populateForm.addEventListener("click", populateFormFromServer);
        personForm.addEventListener("submit", submitForm);
        personForm.addEventListener("reset", resetResult);
    });
})(window, document, $);
