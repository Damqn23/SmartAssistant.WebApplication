$(document).ready(function () {
    // Event handler for keyup in the search box
    $('#search-input').on('keyup', function () {
        var query = $(this).val();  // Get the value entered in the search box

        // Check if the search query is not empty
        if (query.length > 1) {  // Start searching after 2 characters
            $.ajax({
                url: '/Home/SearchSuggestions',  // Send query to this action method
                type: 'GET',
                data: { searchQuery: query },   // The search term
                success: function (response) {
                    // Clear previous results
                    $('#search-results').empty().show();

                    if (response.results.length > 0) {
                        // Populate results
                        response.results.forEach(function (item) {
                            $('#search-results').append(
                                '<a href="' + item.url + '" class="list-group-item list-group-item-action">' +
                                '<strong>' + item.name + '</strong> (' + item.type + ')' +
                                '</a>'
                            );
                        });
                    } else {
                        // Show no results message
                        $('#search-results').append(
                            '<p class="list-group-item text-muted">No results found.</p>'
                        );
                    }
                },
                error: function (error) {
                    console.log('AJAX Error:', error);  // Log any error in the console
                }
            });
        } else {
            $('#search-results').hide(); // Hide the results if query is too short
        }
    });

    // Hide the suggestions dropdown when clicking outside the search box
    $(document).click(function (e) {
        if (!$(e.target).closest('#search-input, #search-results').length) {
            $('#search-results').hide();
        }
    });
});
