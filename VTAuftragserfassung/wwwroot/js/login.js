// Login
function submitLogin() {
    let userId = document.getElementById("userId").value;
    let auth = document.getElementById("auth").value;
    let model = { UserId: userId, Auth: auth };
    let loginAttempt = backendRequest("POST", "/LoginVerification", model);
    if (loginAttempt === "true") {
        window.location.href = "/Home/Dashboard";
    }
}