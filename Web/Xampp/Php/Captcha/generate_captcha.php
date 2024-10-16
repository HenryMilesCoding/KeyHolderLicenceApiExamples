<?php
session_start();

// Funktion zum Abrufen des Schlüssels über die API mit cURL
function getLicenceKeyFromAPI() {
    $curl = curl_init();

    curl_setopt_array($curl, array(
        CURLOPT_URL => 'https://licence.influencers-hub.de/?action=createlicence&userkey=WigYb20i23srt27jauIsIzV5fek64Qtq9&keytype=2&length=8&algo=Nummeric&secret=L1kC!YFT*02jj!!SX$&fireOn=Tst',
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

    // API-Antwort als JSON decodieren und den Wert für "newKey" extrahieren
    $result = json_decode($response, true);

    // "newKey" aus der Backdata extrahieren
    return $result['backdata']['NewKey'] ?? null; // Falls kein Ergebnis vorliegt, wird null zurückgegeben
}

// Funktion zum Erstellen des Captcha-Bildes
function createCaptcha($text) {
    $width = 200;
    $height = 70;

    // Erstelle ein Bild
    $image = imagecreatetruecolor($width, $height);

    // Farben festlegen
    $bgColor = imagecolorallocate($image, 255, 255, 255); // Weißer Hintergrund
    $textColor = imagecolorallocate($image, 0, 0, 0); // Schwarzer Text

    // Fülle den Hintergrund mit der Hintergrundfarbe
    imagefilledrectangle($image, 0, 0, $width, $height, $bgColor);

    // Füge Rauschen hinzu (Verfälschung)
    for ($i = 0; $i < 50; $i++) {
        $noiseColor = imagecolorallocate($image, rand(150, 255), rand(150, 255), rand(150, 255));
        imagesetpixel($image, rand(0, $width), rand(0, $height), $noiseColor);
    }

    // Zeichne zufällige Linien zur weiteren Verfälschung
    for ($i = 0; $i < 5; $i++) {
        $lineColor = imagecolorallocate($image, rand(0, 255), rand(0, 255), rand(0, 255));
        imageline($image, rand(0, $width), rand(0, $height), rand(0, $width), rand(0, $height), $lineColor);
    }

    // Text in das Bild einfügen (mit eingebauter Schriftart)
    $font = 5; // Eingebaute Schriftgröße (1 bis 5 verfügbar)
    $textWidth = imagefontwidth($font) * strlen($text); // Berechnung der Textbreite
    $textHeight = imagefontheight($font); // Berechnung der Texthöhe

    // Positioniere den Text in der Mitte
    $x = ($width - $textWidth) / 2;
    $y = ($height - $textHeight) / 2;

    imagestring($image, $font, $x, $y, $text, $textColor);

    // Setze den Header, um das Bild als PNG zurückzugeben
    header('Content-Type: image/png');
    imagepng($image);
    imagedestroy($image);
}

// API-Aufruf zum Abrufen des Schlüssels
$captchaString = getLicenceKeyFromAPI();

if ($captchaString) {
    // Speichere den Schlüssel in der Session für die spätere Verifizierung
    $_SESSION['captcha'] = $captchaString;

    // Erzeuge das Captcha-Bild mit dem Lizenzschlüssel als Text
    createCaptcha($captchaString);
} else {
    // Falls kein Schlüssel von der API empfangen wurde, Fehlermeldung anzeigen
    echo "Fehler beim Abrufen des Lizenzschlüssels.";
}
?>