"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/signalRServer").build();

function loadPage(pageNumber) {
    fetch(`/NewsArticles/Index?handler=Partial&pageNumber=${pageNumber}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById("newsPartialContainer").innerHTML = html;
        })
        .catch(err => console.error("Error loading partial:", err));
}

connection.start().catch(function (err) {
    return console.error(err.toString());
});

