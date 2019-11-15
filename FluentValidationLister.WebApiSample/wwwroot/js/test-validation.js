(function (window, document, $) {
    // Button, Form, Div
    var populateForm, personForm, resultPanel;

    var ruleList, messageList;

    var validateForm = function () {
        var data = getFormValues();
        var form = $(personForm);
        var errorList = [];

        console.log(data);
        for (var fieldName in ruleList)
        {
            if (fieldName !== "address") {
                var valid = true;
                for (var ruleName in ruleList[fieldName]) {
                    switch (ruleName) {
                        case "required":
                            if (!data[fieldName] || data[fieldName] === "") {
                                valid = false;
                                errorList.push(messageList[fieldName][ruleName]);
                            }
                            break;
                    }
                }

                if (valid === false) $('[name="' + fieldName + '"]', form).addClass("error");
            }
        }

        if (errorList.length > 0)
        {
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

        if (validateForm() === true)
        {
            personForm.disabled = true;

            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = displayFormResult;
            xhr.open("POST", "api/Person", true);
            xhr.setRequestHeader("Content-Type", "application/json");

            xhr.send(JSON.stringify(getFormValues(true)));
        }
    };

    var displayFormResult = function () {
        if (this.readyState === 4) {
            personForm.disabled = false;

            var validationResponse = JSON.parse(this.responseText);
            if (this.status === 400) {
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
            if (field.value !== "") {
                if (splitField === true && field.name.includes(".")) {
                    var split = field.name.split(".");
                    if (!data[split[0]]) data[split[0]] = {};

                    data[split[0]][split[1]] = field.value;

                } else {
                    data[field.name] = field.value;
                }
            }
        });

        // Correct any string-types that should be numeric or Boolean (only one in this example)
        if (data.age === "") data.age = null; else if (data.age) data.age = parseInt(data.age);

        return data;
    };

    var populateFormFromServerRequest = function () {
        resetResult();

        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = populateFormFromServerResponse;
        xhr.open("GET", "api/Person/1", true);
        xhr.setRequestHeader("Accepts", "application/json");
        xhr.send();
    };

    var populateFormFromServerResponse = function () {
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

    var resetResult = function () {
        $(".error").removeClass("error");
        if (resultPanel.hasChildNodes()) resultPanel.removeChild(resultPanel.childNodes[0]);
    };

    var getValidationRules = function () {
        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = setValidationRules;
        xhr.open("POST", "api/Person?validation=true", true);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.send(JSON.stringify({ person: 1 }));
    };

    var setValidationRules = function () {
        if (this.readyState === 4) {
            var validationResponse = JSON.parse(this.responseText);
            ruleList = validationResponse.validatorList;
            messageList = validationResponse.errorList;
        }
    };

    window.addEventListener("load", function () {
        getValidationRules();

        populateForm = document.getElementById("populate-form"); // Button
        personForm = document.getElementById("person-form"); // Form
        resultPanel = document.getElementById("result-panel"); // Div

        populateForm.addEventListener("click", populateFormFromServerRequest);
        personForm.addEventListener("submit", submitForm);
        personForm.addEventListener("reset", resetResult);
    });
})(window, document, $);
