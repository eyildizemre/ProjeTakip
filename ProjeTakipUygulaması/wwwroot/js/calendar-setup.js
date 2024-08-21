document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');

    var events = window.calendarEvents || []; // Dinamik olarak View'dan gelecek events verisi

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        locale: 'tr', // Takvimi Türkçe yapmak için locale ayarı
        buttonText: {
            today: 'Bugün' // "today" butonunu Türkçe'ye çevirir
        },
        events: events,
        // Diğer takvim ayarları buraya eklenebilir
    });

    calendar.render();
});
