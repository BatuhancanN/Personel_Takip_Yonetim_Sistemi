package com.meddata.batuhan.personel_takip_otomasyon

import android.content.pm.PackageManager
import android.graphics.Color
import android.os.Build
import android.os.Bundle
import android.os.Handler
import android.util.Log
import android.widget.TextView
import androidx.activity.enableEdgeToEdge
import androidx.annotation.OptIn
import androidx.annotation.RequiresApi
import androidx.appcompat.app.AppCompatActivity
import androidx.camera.core.CameraSelector
import androidx.camera.core.ExperimentalGetImage
import androidx.camera.core.ImageAnalysis
import androidx.camera.core.ImageProxy
import androidx.camera.core.Preview
import androidx.camera.lifecycle.ProcessCameraProvider
import androidx.camera.view.PreviewView
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.lifecycle.LifecycleOwner
import com.google.android.material.snackbar.Snackbar
import com.google.common.util.concurrent.ListenableFuture
import com.google.mlkit.vision.barcode.BarcodeScanning
import com.google.mlkit.vision.common.InputImage
import com.meddata.batuhan.personel_takip_otomasyon.classes.Personel
import com.meddata.batuhan.personel_takip_otomasyon.data.ApiUtils
import com.meddata.batuhan.personel_takip_otomasyon.data.CRUDcevap
import com.meddata.batuhan.personel_takip_otomasyon.data.PersonelCevap
import com.meddata.batuhan.personel_takip_otomasyon.data.PersonelDAOInterface
import com.meddata.batuhan.personel_takip_otomasyon.databinding.ActivityCameraBinding
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import java.util.concurrent.ExecutorService
import java.util.concurrent.Executors

import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

class CameraActivity : AppCompatActivity() {
    private lateinit var cameraProviderFuture: ListenableFuture<ProcessCameraProvider>
    private lateinit var cameraExecutor: ExecutorService
    private lateinit var txtResult: TextView
    private lateinit var txtSonuc: TextView
    private var isScanningAllowed = true // Tarama kontrolü
    private val handler = Handler() // 3 saniye beklemek için handler

    private lateinit var binding: ActivityCameraBinding
    private lateinit var pdi: PersonelDAOInterface

    lateinit var personelDurum: String


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        binding = ActivityCameraBinding.inflate(layoutInflater)
        setContentView(binding.root)

        checkCameraPermission()
        pdi = ApiUtils.getPersonelDAOInterface(this)

        txtResult = binding.txtResult
        txtResult.text = "QR Bekleniyor..." // Başlangıçta yazı
        txtSonuc = binding.txtQR

        cameraProviderFuture = ProcessCameraProvider.getInstance(this)
        cameraExecutor = Executors.newSingleThreadExecutor()

        cameraProviderFuture.addListener({
            val cameraProvider = cameraProviderFuture.get()
            val preview = Preview.Builder().build().also {
                val previewView = findViewById<androidx.camera.view.PreviewView>(binding.previewView.id)
                previewView.scaleType = PreviewView.ScaleType.FILL_START // Ayna efektini kapat
                it.setSurfaceProvider(previewView.surfaceProvider)
            }


            val imageAnalysis = ImageAnalysis.Builder()
                .setBackpressureStrategy(ImageAnalysis.STRATEGY_KEEP_ONLY_LATEST)
                .build()

            imageAnalysis.setAnalyzer(cameraExecutor, ImageAnalysis.Analyzer { imageProxy ->
                if (isScanningAllowed) {
                    processImage(imageProxy)
                } else {
                    imageProxy.close() // Tarama durduğunda bekleyen görüntüyü kapat
                }
            })

            val cameraSelector = CameraSelector.DEFAULT_FRONT_CAMERA // Ön kamera kullanılacak

            try {
                cameraProvider.unbindAll()
                cameraProvider.bindToLifecycle(this as LifecycleOwner, cameraSelector, preview, imageAnalysis)
            } catch (e: Exception) {
                Log.e("CameraX", "Kamera başlatma hatası: ${e.message}")
            }
        }, ContextCompat.getMainExecutor(this))

    }


    @OptIn(ExperimentalGetImage::class)
    private fun processImage(imageProxy: ImageProxy) {
        val mediaImage = imageProxy.image ?: return
        val image = InputImage.fromMediaImage(mediaImage, imageProxy.imageInfo.rotationDegrees)

        val scanner = BarcodeScanning.getClient()
        scanner.process(image)
            .addOnSuccessListener { barcodes ->
                for (barcode in barcodes) {
                    val rawValue = barcode.rawValue
                    if (rawValue != null) {
                        //txtResult.text = rawValue // QR kodu ekranda göster
                        Log.e("QR Kodu", "Okundu: $rawValue")
                        veriyiAl(rawValue)

                        // Tarama durdurulacak ve 3 saniye bekleme yapılacak
                        isScanningAllowed = false
                        handler.postDelayed({
                            isScanningAllowed = true
                            txtResult.text = "QR Bekleniyor..." // 3 saniye sonra eski haline dön
                            val renk = ContextCompat.getColor(this@CameraActivity,android.R.color.transparent)
                            val renk2 = ContextCompat.getColor(this@CameraActivity,R.color.ikonRenk)
                            txtResult.setTextColor(renk2)
                            txtSonuc.setTextColor(renk2)
                            binding.linearDurum.setBackgroundColor(renk)
                        }, 3000) // 3 saniye bekle
                    }
                }
            }
            .addOnFailureListener {
                Log.e("QR Kodu", "Barkod okuma hatası!", it)
            }
            .addOnCompleteListener {
                imageProxy.close()
            }
    }

    private fun checkCameraPermission() {
        if (ContextCompat.checkSelfPermission(this, android.Manifest.permission.CAMERA)
            != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, arrayOf(android.Manifest.permission.CAMERA), 100)
        }
    }

    fun veriyiAl(veri: String){

        val id = veri.toIntOrNull()

        if (id != null){
            kisiGetir(id)
        }
        else{
            txtResult.text = "STANDARTA UYGUN QR/BARKOD GÖSTERİN"
        }

    }

    fun kisiGetir(id: Int){

        pdi.kisiyiBul(id).enqueue(object : Callback<PersonelCevap> {
            override fun onResponse(call: Call<PersonelCevap>, response: Response<PersonelCevap>) {

                if(response != null){
                    val personelListe = response.body()?.personeller

                    if (personelListe!= null && personelListe.isNotEmpty()){
                        islemYap(personelListe)
                    }
                    else{
                        txtSonuc.text = "Personel Bulunamadı."
                        txtResult.text = "STANDARTA UYGUN QR/BARKOD GÖSTERİN"
                    }
                }


            }

            override fun onFailure(call: Call<PersonelCevap>, t: Throwable) {

            }
        })

    }

    fun islemYap(liste: List<Personel>){

        for (personel in liste){

            txtSonuc.text = "${personel.personel_isim} ${personel.personel_soyisim}"

            if (personel.personel_durum == 1){
                pdi.hareketEkle(personel.personel_id, 0, "${zamanAl()}").enqueue(object : Callback<CRUDcevap>{
                    override fun onResponse(call: Call<CRUDcevap>, response: Response<CRUDcevap>) {

                        if(response != null){
                            val success = response.body()?.success ?: 0
                            val message = response.body()?.message

                            if (success == 1){
                                val renk = ContextCompat.getColor(this@CameraActivity, R.color.cikis)
                                val renk2 = ContextCompat.getColor(this@CameraActivity, R.color.ikonRenk)
                                txtResult.text = "Güle Güle"
                                binding.linearDurum.setBackgroundColor(renk)
                                txtSonuc.setTextColor(renk2)
                            }
                            else{
                                Snackbar.make(binding.root, "${message}",Snackbar.LENGTH_SHORT).show()
                            }
                        }
                        else{
                            Snackbar.make(binding.root, "Response null",Snackbar.LENGTH_SHORT).show()
                        }

                    }

                    override fun onFailure(call: Call<CRUDcevap>, t: Throwable) {
                        Log.e("hareket", t.toString())
                    }


                })

            }
            else if(personel.personel_durum == 0){
                pdi.hareketEkle(personel.personel_id, 1, "${zamanAl()}").enqueue(object : Callback<CRUDcevap>{
                    override fun onResponse(call: Call<CRUDcevap>, response: Response<CRUDcevap>) {

                        if(response != null){
                            val success = response.body()?.success ?: 0
                            val message = response.body()?.message

                            if (success == 1){
                                val renk = ContextCompat.getColor(this@CameraActivity, R.color.giris)
                                val renk2 = ContextCompat.getColor(this@CameraActivity, R.color.ikonRenk)
                                txtResult.text = "Hoş Geldiniz"
                                binding.linearDurum.setBackgroundColor(renk)
                                txtSonuc.setTextColor(renk2)
                            }
                            else{
                                Snackbar.make(binding.root, "${message}",Snackbar.LENGTH_SHORT).show()
                            }
                        }
                        else{
                            Snackbar.make(binding.root, "Response null",Snackbar.LENGTH_SHORT).show()
                        }

                    }

                    override fun onFailure(call: Call<CRUDcevap>, t: Throwable) {
                        Log.e("hareket", t.toString())
                    }


                })
            }
            else{
                Snackbar.make(binding.root, "İşlem Başarısız",Snackbar.LENGTH_SHORT).show()
            }

        }

    }

    @RequiresApi(Build.VERSION_CODES.O)
    fun zamanAl() : String {
        // Anlık tarih ve saat bilgisini al
        val currentDateTime = LocalDateTime.now()

        // İsteğe bağlı: Formatlama için bir şablon belirle
        val formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss")
        val formattedDateTime = currentDateTime.format(formatter)

        // Sonuçları yazdır
        return "${formattedDateTime.toString()}" ?: ""
    }

    override fun onDestroy() {
        super.onDestroy()
        cameraExecutor.shutdown()
    }
}