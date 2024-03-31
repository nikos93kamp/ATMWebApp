
$(document).ready(function () {
    $('#searchButton').click(function () {
        var searchValue = $('#searchInput').val().toLowerCase();
        $('.grid-view tbody tr').each(function () {
            var rowText = $(this).text().toLowerCase();
            if (rowText.includes(searchValue)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });
});

$(document).ready(function () {
    $('.grid-row').click(function () {
        if (!$(this).hasClass('selected')) {
            $('.grid-row').removeClass('selected');
            $(this).addClass('selected');
        } else {
            $(this).removeClass('selected');
        }
    });

    $('#validateButton').click(function (e) {
        e.preventDefault();

        var selectedRow = $('.grid-row.selected');
        var pin = $('#pinInput').val();

        if (selectedRow.length > 0) {
            var userId = selectedRow.data('id');
            // Perform database validation using AJAX or any other method
            $.ajax({
                url: '/ATM/ForgotPIN/',
                method: 'POST',
                data: { userId: userId, pin: pin },
                success: function (response) {
                    if (response.isValid) {
                        window.location.href = '/ATM/ShowMessage/';
                    }
                    else {
                        if (pin === '') {
                            // Reset GridView and display error message
                            $('#searchInput').val('');
                            $('.grid-row').removeClass('selected');
                            $('#pinInput').val('');
                            $('#errorContainer').removeClass('text-danger').text('');
                            // Hide the toastr error message
                            setTimeout(function () {
                                $('.toast-error').fadeOut();
                            }, 0);
                            // Display the error message
                            $('#errorContainer').addClass('text-danger').text(response.errors[0]);
                        }
                        if (!$.isNumeric(pin)) {
                            // Reset GridView and display error message
                            $('#searchInput').val('');
                            $('.grid-row').removeClass('selected');
                            $('#pinInput').val('');
                            $('#errorContainer').removeClass('text-danger').text('');
                            setTimeout(function () {
                                $('.toast-error').fadeOut();
                            }, 0);
                            //$('#pinInput').html(response.errors[0]);
                            toastr.error(response.errors[0], '', { "positionClass": "toast-top-center1", });
                        }
                        else if (pin.length !== 4) {
                            $('#searchInput').val('');
                            $('.grid-row').removeClass('selected');
                            $('#pinInput').val('');
                            $('#errorContainer').removeClass('text-danger').text('');
                            //$('#pinInput').html(response.errors[0]);
                            setTimeout(function () {
                                $('.toast-error').fadeOut();
                            }, 0);
                            toastr.error(response.errors[0], '', { "positionClass": "toast-top-center1", });
                        }

                        if (response.isInValid) {
                            window.location.href = '/ATM/ShowMessage?useCustomMessage=true';
                        }

                        if (response.isSuccess) {
                            $('#searchInput').val('');
                            $('.grid-row').removeClass('selected');
                            $('#pinInput').val('');
                            $('#errorContainer').removeClass('text-danger').text('');
                            toastr.success('Successful Transaction!', '', { "positionClass": "toast-top-center1", });
                        }

                        else {
                            $('#searchInput').val('');
                            $('.grid-row').removeClass('selected');
                            $('#pinInput').val('');
                            $('#errorContainer').addClass('text-danger').text(response.errors[0]);
                        }
                    }
                }
            });
        }
    });
});

$(document).ready(function () {
    $('#validateButton').click(function (e) {
        e.preventDefault();

        var enteredPin = $('#pinInput').val();
        var userId = null; // Set userId to null

        if (userId === null) {
            // Perform database validation using AJAX or any other method
            $.ajax({
                url: '/ATM/CheckPIN/',
                method: 'POST',
                data: { enteredPin: enteredPin },
                success: function (response) {
                    if (response.isValid) {
                        // window.location.href = '/ATM/ShowMessage/';$('#searchInput').val('');
                    } else {
                        if (enteredPin === '') {
                            // Reset GridView and display error message
                            $('#searchInput').val('');
                            $('#pinInput').val('');
                            $('#errorContainer').addClass('text-danger').text(response.errors[0]);
                        }

                        if (response.isInValid) {
                            $('#searchInput').val('');
                            $('#pinInput').val('');
                            $('#errorContainer').removeClass('text-danger').text('');
                            toastr.error("You must select a CardNumber", '', { "positionClass": "toast-top-center1", });
                        }

                        else {
                            $('#searchInput').val('');
                            $('.grid-row').removeClass('selected');
                            $('#pinInput').val('');
                            //toastr.error(response.errors[0], '', { "positionClass": "toast-top-center1", });
                        }
                    }
                }
            });
        }
    });
});


    //$(document).ready(function () {
    //    var currentPage = 1;
    //var pageSize = 10;
    //var totalRows = $('.grid-view tbody tr').length;
    //var totalPages = Math.ceil(totalRows / pageSize);

    //function showPage(page) {
    //        var startIndex = (page - 1) * pageSize;
    //var endIndex = startIndex + pageSize;

    //$('.grid-view tbody tr').hide();
    //$('.grid-view tbody tr').slice(startIndex, endIndex).show();
    //    }

    //$('.previous-page').click(function () {
    //        if (currentPage > 1) {
    //    currentPage--;
    //showPage(currentPage);
    //        }
    //    });

    //$('.next-page').click(function () {
    //        if (currentPage < totalPages) {
    //    currentPage++;
    //showPage(currentPage);
    //        }
    //    });

    //$('.first-page').click(function () {
    //    currentPage = 1;
    //showPage(currentPage);
    //    });

    //$('.last-page').click(function () {
    //    currentPage = totalPages;
    //showPage(currentPage);
    //    });

    //showPage(currentPage);
    //});


$(document).ready(function () {
    var defaultOrder = $('tbody > tr').get(); // Store default row order

    $('th').click(function () {
        var column = $(this).index();
        var rows = $('tbody > tr').get();

        rows.sort(function (a, b) {
            var A = $(a).children('td').eq(column).text().toUpperCase();
            var B = $(b).children('td').eq(column).text().toUpperCase();
            return A.localeCompare(B);
        });

        if ($(this).hasClass('asc')) {
            rows.reverse();
            $(this).removeClass('asc').addClass('desc');
        } else {
            $(this).removeClass('desc').addClass('asc');
        }

        $('tbody').append(rows);
    });

    // Reset to default sorting on double-click
    $('th').dblclick(function () {
        $('th').removeClass('asc desc'); // Remove sorting classes
        $('tbody').append(defaultOrder); // Revert to default order
    });
});
