<?php

$response = array();

if(isset($_POST['personel_id']))
{

    $personel_id = $_POST['personel_id'];

    require_once __DIR__ . '/db_config.php';

    $baglanti = mysqli_connect(DB_SERVER, DB_USER, DB_PASSWORD, DB_DATABASE);

    if(!$baglanti)
    {
        die("Bağlantı Hatası!" . mysqli_connect_error());
    }

    $sqlSorgu = "SELECT 
                    personel_id, personel_isim, personel_soyisim, personel_pozisyon, personel_durum
                FROM
                    personeller
                WHERE
                    personel_id = $personel_id";

    $result = mysqli_query($baglanti, $sqlSorgu);

    if(mysqli_num_rows($result) > 0)
    {
        $response["personeller"] = array();

        while($row = mysqli_fetch_assoc($result))
        {
            $personeller = array();

            $personeller["personel_id"] = $row["personel_id"];
            $personeller["personel_isim"] = $row["personel_isim"];
            $personeller["personel_soyisim"] = $row["personel_soyisim"];
            $personeller["personel_pozisyon"] = $row["personel_pozisyon"];
            $personeller["personel_durum"] = $row["personel_durum"];

            array_push($response["personeller"], $personeller);

        }

        $response["success"] = 1;
        echo json_encode($response);
    }
    else
    {
        $response["success"] = 0;
        $response["message"] = "Veri Bulunamadı";
        echo json_encode($response);
    }

    mysqli_close($baglanti);

}
else
{ 
    $response["success"] = 0;
    $response["message"] = "Gerekli Alanlar Eksik";
    echo json_encode($response);
}

?>
