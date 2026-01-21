(function() {
    if (!localStorage.getItem("deviceId"))
    {
        const id =
            crypto.randomUUID?.() ||
            Math.random().toString(36).substring(2) +
            Date.now().toString(36);

        localStorage.setItem("deviceId", id);
    }
})();
