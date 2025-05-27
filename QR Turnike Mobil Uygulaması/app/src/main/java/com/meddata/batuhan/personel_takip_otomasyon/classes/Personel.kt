package com.meddata.batuhan.personel_takip_otomasyon.classes

import com.google.gson.annotations.Expose
import com.google.gson.annotations.SerializedName
import java.io.Serializable

data class Personel(

    @SerializedName("personel_id")
    @Expose
    var personel_id: Int,

    @SerializedName("personel_isim")
    @Expose
    var personel_isim: String,

    @SerializedName("personel_soyisim")
    @Expose
    var personel_soyisim: String,

    @SerializedName("personel_pozisyon")
    @Expose
    var personel_pozisyon: String,

    @SerializedName("personel_durum")
    @Expose
    var personel_durum: Int,

): Serializable