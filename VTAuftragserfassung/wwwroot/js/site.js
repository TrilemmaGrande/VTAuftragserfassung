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

// Login
function submitLogin() {
    let userId = document.getElementById("userId").value;
    let auth = document.getElementById("auth").value;
    let model = { UserId: userId, Auth: auth };
    let htmlResponse = backendRequest("POST", "/LoginVerification", model);
    document.open();
    document.write(htmlResponse);
    document.close();
}
// Assignments
function onEnterPressButton(buttonId, e) {
    if (e.keyCode === 13) {
        document.getElementById(buttonId).click();
    }
}
function toggleAssignmentDetails(rowId) {
    let rowPartialDiv = document.getElementById("partialAssignmentDetailsToggled_" + rowId)
    if (rowPartialDiv.innerHTML == "") {
        let model = rowPartialDiv.dataset.avm;
        rowPartialDiv.innerHTML = backendRequest("POST", "/Home/GetAssignmentDetailsPartial/", model);
    }
    else {
        rowPartialDiv.innerHTML = "";
    }
}