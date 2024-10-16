<!DOCTYPE html>
<html lang="de">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Verify Page</title>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
</head>
<body>
    <div style="text-align: center;">
        <h1>Captcha Verification</h1>
        <!-- Bild (Captcha) -->
        <img id="captchaImage" src="generate_captcha.php" alt="Captcha">
        <br>
        <button onclick="reloadCaptcha()">Reload Captcha</button>
        <br><br>
        <!-- Eingabefeld -->
        <input type="text" id="userInput" placeholder="Enter the image text">
        <br><br>
        <!-- Verify Button -->
        <button onclick="verifyCaptcha()">Verify</button>
        <br><br>
        <!-- Ergebnis -->
        <div id="result"></div>
    </div>

    <script>
        function reloadCaptcha() {
            // Das Captcha-Bild neu laden
            document.getElementById('captchaImage').src = 'generate_captcha.php?' + Date.now();
        }

        function verifyCaptcha() {
            var userInput = document.getElementById('userInput').value;

            $.ajax({
                url: 'verify.php',
                type: 'POST',
                data: {user_input: userInput},
                success: function(response) {
                    var result = JSON.parse(response);
                    if (result.status === 'success') {
                        document.getElementById('result').innerText = result.message;
                    } else {
                        document.getElementById('result').innerText = result.message;
                    }
                }
            });
        }
    </script>
</body>
</html>