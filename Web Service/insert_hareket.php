<?php

$response = array();

if(isset($_POST['h_personel_id']) && isset($_POST['h_durumu']) && isset($_POST['h_tarih']))
{
    $h_personel_id = $_POST['h_personel_id'];
    $h_durumu = $_POST['h_durumu'];
    $h_tarih = $_POST['h_tarih'];

    require_once __DIR__ . '/db_config.php';

    $baglanti = mysqli_connect(DB_SERVER, DB_USER, DB_PASSWORD, DB_DATABASE);

    if(!$baglanti)
    {
        die("Bağlantı Hatası! " . mysqli_connect_error());
    }

    // SQL Sorgularını ayrı ayrı çalıştır
    $sqlInsert = "INSERT INTO hareketler (h_personel_id, h_durumu, h_tarih) VALUES ($h_personel_id, $h_durumu, '$h_tarih')";
    $sqlUpdate = "UPDATE personeller SET personel_durum = $h_durumu WHERE personel_id = $h_personel_id";

    if(mysqli_query($baglanti, $sqlInsert) && mysqli_query($baglanti, $sqlUpdate))
    {
        $response["success"] = 1;
        $response["message"] = "API Başarılı";
    }
    else
    {
        $response["success"] = 0;
        $response["message"] = "SQL Hatası: " . mysqli_error($baglanti); // Hata mesajını ekledik
    }

    echo json_encode($response);
    mysqli_close($baglanti);
}
else
{ 
    $response["success"] = 0;
    $response["message"] = "Gerekli Alanlar Eksik";
    echo json_encode($response);
}


?>
