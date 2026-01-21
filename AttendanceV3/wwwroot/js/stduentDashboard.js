const connection = new signalR.HubConnectionBuilder()
    .withUrl("/attendanceHub")
    .build();

connection.start().then(() => console.log("SignalR connected"))
    .catch(err => console.error(err.toString()));

connection.on("ReceiveAttendanceUpdate", (data) => {
    const table = document.getElementById("attendanceTable");
    const row = document.createElement("tr");
    row.innerHTML = `
        <td>${data.subject}</td>
        <td>${data.yearLevel}</td>
        <td>${data.classSection}</td>
        <td>${new Date(data.timeIn).toLocaleString()}</td>
    `;
    table.prepend(row);
});
