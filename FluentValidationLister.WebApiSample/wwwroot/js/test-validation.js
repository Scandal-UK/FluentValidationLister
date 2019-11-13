(function (window, document, $) {
    // Button, Form, Div
    var populateForm, personForm, resultPanel;

    var submitForm = function (e) {
        // Forget the form's usual submit behaviour and ignore concurrent submits
        e.preventDefault();
        if (personForm.disabled === true) return;

        // Disable the submit button and remove any previous results
        personForm.disabled = true;
        if (resultPanel.hasChildNodes()) resultPanel.removeChild(resultPanel.childNodes[0]);

        // Gather the form fields into an array (I'm cheating here and using jQuery)
        var data = {};
        $(personForm).serializeArray().map(function (field) {
            // Hint: Here we're only looking for one level of children (address); to make this
            // re-usable you would need to implement something more recursive!
            if (field.value !== "") {
                if (field.name.includes(".")) {
                    // Split from [ChildArrayName].[FieldName]
                    var split = field.name.split(".");

                    // Create the child array if it does not exist
                    if (!data[split[0]]) data[split[0]] = {};

                    // Add the value to the child array
                    data[split[0]][split[1]] = field.value;

                } else {
                    // Add the field value to the array
                    data[field.name] = field.value;
                }
            }
        });

        // Correct any string-types that should be numeric or Boolean
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

            var code = document.createElement("code");
            code.classList.add("language-json");
            code.innerHTML = JSON.stringify(validationResponse, null, 2);

            var output = document.createElement("pre");
            if (this.status >= 400) output.classList.add("error");
            output.appendChild(code);
            resultPanel.appendChild(output);
        }
    };

    var populateFormFromServerRequest = function () {
        // Perform a GET request
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

            // As I'm already using jQuery in this file, I use it here to make this code shorter ;)
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

        populateForm.addEventListener("click", populateFormFromServerRequest);
        personForm.addEventListener("submit", submitForm);
    });
})(window, document, $);
