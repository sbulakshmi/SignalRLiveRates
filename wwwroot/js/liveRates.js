"use strict";
const endpoint = "api/resource";
var connection = new signalR.HubConnectionBuilder().withUrl("/liveRatesHub").build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveRates", function (rates) {
    var li = document.createElement("li");
    document.getElementById("liveRatesList").appendChild(li);

    var currentdate = new Date();
    var datetime = "Last Sync: " + currentdate.getDate() + "/"
        + (currentdate.getMonth() + 1) + "/"
        + currentdate.getFullYear() + " @ "
        + currentdate.getHours() + ":"
        + currentdate.getMinutes() + ":"
        + currentdate.getSeconds();

    li.textContent = `${datetime} fetched ${rates}`;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    connection.invoke("GetRates").catch(function (err) {
        return console.error(err.toString());
    });
});