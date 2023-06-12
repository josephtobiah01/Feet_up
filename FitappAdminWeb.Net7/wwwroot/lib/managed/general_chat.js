function htmlToElement(html) {
    var template = document.createElement("template");
    html = html.trim();
    template.innerHTML = html;
    return template.content.firstChild;
}

function chat_init(username) {
    var currentName = username;
    var messageInput = document.getElementById("message");

    window.FitApp.Chat.InitClient("/chathub", username);

    window.FitApp.Chat.Connection.on('broadcastMessage', function (name, message) {
        var encodedName = name;
        var encodedMsg = message;

        var isOther = currentName != name; //Math.floor(Math.random() * 100) % 2 == 0;
        var chatCard_html = "";

        if (isOther) {
            chatCard_html = document.getElementById("otherchat_tmpl").innerHTML;
        }
        else {
            chatCard_html = document.getElementById("mychat_tmpl").innerHTML;
        }
        chatCard_html = chatCard_html.replace("{IMAGEURL}", "");
        chatCard_html = chatCard_html.replace("{USERNAME}", encodedName);
        chatCard_html = chatCard_html.replace("{MESSAGE}", encodedMsg);
        chatCard_html = chatCard_html.replace("{ROLE}", "Coach");

        var chatCard_Element = htmlToElement(chatCard_html);
        document.getElementById("chatbox").appendChild(chatCard_Element);
    });

    window.FitApp.Chat.Connection.start().then(function () {
        console.log("Connection Started");
        document.getElementById("sendmessage").addEventListener("click", function (event) {
            var name = window.FitApp.Chat.UserName;
            window.FitApp.Chat.Connection.invoke("send", name, messageInput.value);

            messageInput.value = "";
            messageInput.focus();
            event.preventDefault();
        });
    })
    .catch(error => {
        console.log(error);
    });
}