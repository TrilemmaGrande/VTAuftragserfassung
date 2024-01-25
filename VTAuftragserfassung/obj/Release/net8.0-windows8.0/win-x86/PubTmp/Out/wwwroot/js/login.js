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