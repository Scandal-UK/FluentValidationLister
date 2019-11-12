﻿(function (window, document, $) {
    var populateForm, personForm, resultPanel;

    var displayResult = function () {
        if (this.readyState === 4) {
            personForm.disabled = false;

            var validationResponse = JSON.parse(this.responseText);

            var code = document.createElement("code");
            code.classList.add("language-json");
            code.innerHTML = JSON.stringify(validationResponse, null, 2);

            var output = document.createElement("pre");
            if (this.status >= 400) output.classList.add("error");
            output.appendChild(code);
            resultPanel.appendChild(output);
        }
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
        populateForm = document.getElementById("populate-form");
        personForm = document.getElementById("person-form");
        resultPanel = document.getElementById("result-panel");

        populateForm.addEventListener("click", function () {
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = populateFormFromServerResponse;
            xhr.open("GET", "api/Person/1", true);
            xhr.setRequestHeader("Accepts", "application/json");
            xhr.send();
        });

        personForm.addEventListener("submit", function (e) {
            e.preventDefault();
            personForm.disabled = true;
            if (resultPanel.hasChildNodes()) {
                resultPanel.removeChild(resultPanel.childNodes[0]);
            }

            // Gather the form fields into an array
            var data = {};
            $(personForm).serializeArray().map(function (field) {
                if (field.name.includes("."))
                {
                    var split = field.name.split(".");
                    if (!data[split[0]]) data[split[0]] = {};

                    data[split[0]][split[1]] = field.value;
                } else {
                    data[field.name] = field.value;
                }
            });

            // Correct any string-types that should be numeric or Boolean
            if (data.age === "") data.age = null; else data.age = parseInt(data.age);

            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = displayResult;
            xhr.open("POST", "api/Person", true);
            xhr.setRequestHeader("Content-Type", "application/json");

            xhr.send(JSON.stringify(data));
        });
    });
})(window, document, $);
