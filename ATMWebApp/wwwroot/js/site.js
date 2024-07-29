// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
/* site.js */


$(function () {
    $('#loginForm').on('submit', function (e) {
            e.preventDefault(); // Prevent form submission

            //var formData = $(this).serialize();
            
            var cardNumber = $('#cardNumber').val();
            var pin = $('#pin').val();

            //if (typeof pin !== 'number' && (pin % 1 !== 0)) {
            //    $('#validationMessages1').html('');
            //    $('#validationMessages2').html('PIN is not an integer');
            //    toastr.error('PIN is not integer', '', { "positionClass": "toast-top-center1", });
            //    $('#loginForm')[0].reset();
            //    return;
            //}

            // If validation passes, submit the form using AJAX
            $.ajax({
                url: '/ATM/Login',
                //url: $(this).attr('action'),
                type: 'POST',
                //type: $(this).attr('method'),
                data: { CardNumber: cardNumber, PIN: pin },
                //data: formData,
                success: function (response) {
                    // Handle the response from the server
                    if (response.success) {
                        // Handle successful login
                        //toastr.success('Login successful!', '', { "positionClass": "toast-top-center1", });
                        //// If validation passes, redirect to the Options page
                        //setTimeout(function () {
                        //    window.location.href = '/ATM/Options/';
                        //}, 2000); // Redirect after 2 seconds
                        //or
                        // Use Promise for delay
                        //const delay = ms => new Promise(resolve => setTimeout(resolve, ms));

                        //// Chain promises for sequential execution
                        //delay(2000).then(() => {
                        //    window.location.href = 'newpage.html';
                        //});
                        // Display Toastr message and redirect on close
                        //toastr.info('Message to display', '', {
                        //    onHidden: function () {
                        //        window.location.href = 'new-page-url';
                        //    }
                        //});

                        async function redirectToAnotherPage() {
                            toastr.success('Login successful!', '', { "positionClass": "toast-top-center1", });
                            await new Promise(resolve => setTimeout(resolve, 2000)); // Simulating a delay
                            window.location.href = '/ATM/Options/';
                        }
                        // Call the function to redirect to a new page
                        redirectToAnotherPage()
                            .then(() => {
                                console.log("Page redirected successfully");
                            })
                            .catch((error) => {
                                console.error("Error redirecting to the new page: " + error);
                            });
                    }
                    else {
                        // Login failed, display error messages
                        // Display server-side validation errors
                        if (cardNumber === '') {
                            $('#validationMessages1').html(response.errors[0]);
                            $('#validationMessages2').html('');
                            toastr.error(response.errors[0], '', { "positionClass": "toast-top-center1", });
                            $('#loginForm')[0].reset();
                        }
                        else if (pin === '') {
                            $('#validationMessages1').html('');
                            $('#validationMessages2').html(response.errors[0]);
                            toastr.error(response.errors[0], '', { "positionClass": "toast-top-center1", });
                            $('#loginForm')[0].reset();
                        }

                        else if (!$.isNumeric(pin)) {
                            $('#validationMessages1').html('');
                            $('#validationMessages2').html(response.errors[0]);
                            toastr.error(response.errors[0], '', { "positionClass": "toast-top-center1", });
                            $('#loginForm')[0].reset();
                        }

                        else if (cardNumber.length !== 16 && pin.length !== 4) {
                            $('#validationMessages1').html(response.errors[0]);
                            toastr.error(response.errors[0], '', { "positionClass": "toast-top-center1", });
                            $('#validationMessages2').html(response.errors[1]);
                            toastr.error(response.errors[1], '', { "positionClass": "toast-top-center1", });
                            $('#loginForm')[0].reset();
                        }

                        else if (cardNumber.length !== 16) {
                            $('#validationMessages1').html(response.errors[0]);
                            toastr.error(response.errors[0], '', { "positionClass": "toast-top-center1", });
                            $('#validationMessages2').html('');
                            $('#loginForm')[0].reset();
                        }

                        else if (pin.length !== 4) {
                            $('#validationMessages1').html('');
                            $('#validationMessages2').html(response.errors[0]);
                            toastr.error(response.errors[0], '', { "positionClass": "toast-top-center1", });
                            $('#loginForm')[0].reset();
                        }

                        else if (!response.pinExists && !response.cardNumberExists) {
                            // Card number and PIN does not exist in the database
                            $('#validationMessages1').html('CardNumber not exists');
                            toastr.error('CardNumber not exists', '', { "positionClass": "toast-top-center1", });
                            $('#validationMessages2').html('PIN not exists');
                            toastr.error('PIN not exists', '', { "positionClass": "toast-top-center1", });
                            $('#loginForm')[0].reset();
                        }
                        else if (!response.cardNumberExists) {
                            // Card number does not exist in the database
                            $('#validationMessages1').html('CardNumber not exists');
                            toastr.error('CardNumber not exists', '', { "positionClass": "toast-top-center1", });
                            $('#validationMessages2').html('');
                            $('#loginForm')[0].reset();
                        }
                        else if (!response.pinExists) {
                            // PIN does not exist in the database
                            $('#validationMessages1').html('');
                            $('#validationMessages2').html('PIN not exists');
                            toastr.error('PIN not exists', '', { "positionClass": "toast-top-center1", });
                            $('#loginForm')[0].reset();
                        }

                        if (response.logged_user == null) {
                            if (response.pinExists && response.cardNumberExists) {
                                // Actions to be taken when logged_user is null and cardNumber and pin exist
                                // Clear validation messages
                                $('#validationMessages1').html('');
                                $('#validationMessages2').html('');
                                // Display error messages using toastr
                                toastr.error('Wrong combination of CardNumber and PIN', '', { "positionClass": "toast-top-center1" });
                                // Reset the login form
                                $('#loginForm')[0].reset();
                            }
                        }

                        else {
                            toastr.error(response.errors[0], '', { "positionClass": "toast-top-center1", });
                            $('#loginForm')[0].reset();
                        }
                    }
                }
            });
    });
});



// Function to display toast notifications
//function showToast(message) {
//    var toast = $('<div class="toast" role="alert" aria-live="assertive" aria-atomic="true" style="left: 50%; position: fixed;transform: translate(-50%, 0px);z-index: 9999; bottom:80%"></div>');
//    var toastBody = $('<div class="toast-body text-danger"></div>').text(message);
//    toast.append(toastBody);

//    $('.toast-container').append(toast);
//    toast.toast({ delay: 3000 });
//    toast.toast('show');
//}


$(function () {
    $('#withdraw-button').on('click', function (e) {
        e.preventDefault();
        var amount = $('#amount').val();
        $.ajax({
            url: '/FastCash/FastWithdraw',
            type: 'POST',
            data: { Amount: amount },
            success: function () {
                alert('Withdrawal successful!');
                window.location.href = '/ATM/Options/';
            },
            error: function () {
                alert('Withdrawal failed!');
            }
        });
    });
});


