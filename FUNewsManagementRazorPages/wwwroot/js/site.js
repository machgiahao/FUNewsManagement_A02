"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/signalRServer").build();

connection.on("LoadAllItems", function () {
    fetch('/NewsArticles/Index?handler=Partial')
        .then(response => response.text())
        .then(html => {
            document.getElementById("newsListContainer").innerHTML = html;
        })
        .catch(err => console.error("Error loading partial:", err));
});


connection.start().catch(function (err) {
    return console.error(err.toString());
});

