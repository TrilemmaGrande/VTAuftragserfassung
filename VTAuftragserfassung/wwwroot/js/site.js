// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function backendRequest(requestMethod, requestURL, data) {
    let xhr = new XMLHttpRequest();
    xhr.open(requestMethod, requestURL, false);
    xhr.setRequestHeader("Accept", "application/json");
    xhr.setRequestHeader("Content-type", "application/json");
    xhr.send(JSON.stringify(data));

    if (xhr.status == 200) {
        return xhr.responseText;
    }
}

function onEnterPressButton(buttonId, e) {
    if (e.keyCode === 13) {
        document.getElementById(buttonId).click();
    }
}