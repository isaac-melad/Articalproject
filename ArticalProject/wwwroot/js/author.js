$(document).ready(function () {
    // Initialize Bootstrap tooltips
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))

    // Enhanced search with debounce
    let searchTimeout;
    $('#searchInput').on('input', function() {
        clearTimeout(searchTimeout)
        searchTimeout = setTimeout(() => {
            const value = $(this).val().toLowerCase()
            $('table tbody tr').each(function() {
                const text = $(this).text().toLowerCase()
                $(this).toggle(text.includes(value))
            })
        }, 300)
    })

    // Hover effects
    $('.btn').hover(
        function() { $(this).addClass('shadow-sm').css('transform', 'translateY(-1px)') },
        function() { $(this).removeClass('shadow-sm').css('transform', 'none') }
    )
})
