(function () {
    if (window.FitApp === undefined) {
        window.FitApp = {};
    }
    window.FitApp.Chat = {};

    window.FitApp.Chat.InitClient = function (url, username) {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(url)
            .configureLogging(signalR.LogLevel.Information)
            .build();

        window.FitApp.Chat.Connection = connection;
        window.FitApp.Chat.UserName = username;
    };

    window.FitApp.Chat.SendMessage = async function (message, user) {
        var methodName = "SendMessage";

        try {
            var connection = window.FitApp.Chat.Connection;
            if (connection != null) {
                await connection.invoke(methodName, user, message);
            }
            else {
                console.log("Client not initialized");
            }
        }
        catch (err) {
            console.log(err);
        }
    }
})();