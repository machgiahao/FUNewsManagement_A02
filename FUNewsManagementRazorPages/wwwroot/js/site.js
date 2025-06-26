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

connection.on("LoadAllItems", function () {
    const currentPage = 1; 
    loadPage(currentPage);
});

connection.start()
    //.then(() => console.log("SignalR connected"))
    .catch(function (err) {
        return console.error(err.toString());
    });

