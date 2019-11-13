(function (window, document, $) {
    // Button, Form, Div
    var populateForm, personForm, resultPanel;

    var submitForm = function (e) {
        e.preventDefault();
        if (personForm.disabled === true) return;

        personForm.disabled = true;
        $(".error").removeClass("error");
        if (resultPanel.hasChildNodes()) resultPanel.removeChild(resultPanel.childNodes[0]);

        var data = {};
        $(personForm).serializeArray().map(function (field) {
            if (field.value !== "") {
                if (field.name.includes(".")) {
                    var split = field.name.split(".");
                    if (!data[split[0]]) data[split[0]] = {};

                    data[split[0]][split[1]] = field.value;

                } else {
                    data[field.name] = field.value;
                }
            }
        });

        // Correct any string-types that should be numeric or Boolean (only one in this example)
        if (data.age === "") data.age = null; else data.age = parseInt(data.age);

        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = displayFormResult;
        xhr.open("POST", "api/Person", true);
        xhr.setRequestHeader("Content-Type", "application/json");

        xhr.send(JSON.stringify(data));
    };

    var displayFormResult = function () {
        if (this.readyState === 4) {
            personForm.disabled = false;

            var validationResponse = JSON.parse(this.responseText);
            if (this.status === 400) {
                if (console.error) console.error(validationResponse);

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

    var populateFormFromServerRequest = function () {
        $(".error").removeClass("error");
        if (resultPanel.hasChildNodes()) resultPanel.removeChild(resultPanel.childNodes[0]);

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

    window.addEventListener("load", function () {
        populateForm = document.getElementById("populate-form"); // Button
        personForm = document.getElementById("person-form"); // Form
        resultPanel = document.getElementById("result-panel"); // Div

        populateForm.addEventListener("click", populateFormFromServerRequest);
        personForm.addEventListener("submit", submitForm);
    });
})(window, document, $);
