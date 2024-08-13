document.addEventListener('DOMContentLoaded', function() {
    let currentDateTimeLabel = document.getElementById('currentDateTime');

    function updateDateTime() {
        let currentDate = new Date();
        let formattedDate = currentDate.toLocaleDateString() + " " + currentDate.toLocaleTimeString();
        currentDateTimeLabel.textContent = formattedDate;
    }

    // Llama a la función de inmediato para configurar la fecha/hora inicial
    updateDateTime();

    // Establece un intervalo para llamar a la función cada 1.000 milisegundos (1 seg)
    setInterval(updateDateTime, 1000);
});