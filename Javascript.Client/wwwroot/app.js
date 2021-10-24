document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);
document.getElementById("logout").addEventListener("click", logout, false);

var config = {
	authority: "https://localhost:5001",
	client_id: "javascript.imagegalleryclient",
	client_secret: "511536EF-F270-4058-80CA-1C89C192F69A",
	response_type: "code",
	scope: "openid profile email address imagegalleryapi roles read edit write delete",
	redirect_uri: "http://localhost:5003/callback.html",
	post_logout_redirect_uri: "http://localhost:5003/index.html"
};

var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {
	if (user) { log(user.profile); }
});

function login() {
	mgr.signinRedirect();
}

function api() {
	mgr.getUser().then(function (user) {
		if (user) {
			//log("User logged in", user.profile);
			var url = "https://localhost:5001/connect/userinfo";

			var xhr = new XMLHttpRequest();
			xhr.open("GET", url);
			xhr.onload = function () {
				log(xhr.status, JSON.parse(xhr.responseText));
			}
			xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
			xhr.send();
		}
		else {
			log("User not logged in");
		}
	});
}

function logout() {
	mgr.signoutRedirect();
}

function log() {
	document.getElementById('results').innerText = '';

	Array.prototype.forEach.call(arguments, function (msg) {
		if (msg instanceof Error) {
			msg = "Error: " + msg.message;
		}
		else if (typeof msg !== 'string') {
			msg = JSON.stringify(msg, null, 2);
		}
		document.getElementById('results').innerHTML += msg + '\r\n';
	});
}