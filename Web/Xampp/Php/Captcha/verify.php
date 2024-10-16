<?php
session_start();

// Funktion zur Verifizierung des Benutzereingangs mit der API
function verifyLicenceWithAPI($userInput) {
    $baseURL = 'https://licence.influencers-hub.de/api.php';
    $action = 'readlicence';
    $publicKey = '681320003074324845689881207848678'; // Publicly used key
    $secret = 'L1kC!YFT*02jj!!SX$';
    $fireOn = 'Tst';

    $params = http_build_query([
        'action' => $action,
        'key' => $userInput,
        'publicKey' => $publicKey,
        'secret' => $secret,
        'fireOn' => $fireOn
    ]);

    $fullURL = $baseURL . '?' . $params;

    $curl = curl_init();

    curl_setopt_array($curl, array(
        CURLOPT_URL => $fullURL,
        CURLOPT_RETURNTRANSFER => true,
        CURLOPT_ENCODING => '',
        CURLOPT_MAXREDIRS => 10,
        CURLOPT_TIMEOUT => 0,
        CURLOPT_FOLLOWLOCATION => true,
        CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
        CURLOPT_CUSTOMREQUEST => 'GET',
    ));

    $response = curl_exec($curl);
    curl_close($curl);

    return json_decode($response, true);
}


//When the form has been sent (POST method)
if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $userInput = $_POST['user_input'];

    // Check whether the user has entered the correct captcha code (and call the API for validation)
    $apiResponse = verifyLicenceWithAPI($userInput);

    if ($apiResponse && $apiResponse['state'] == 1) {
        echo json_encode(['status' => 'success', 'message' => 'Verification successful!']);
    } else {
        echo json_encode(['status' => 'error', 'message' => 'Verification failed. Try again.']);
    }
}
?>