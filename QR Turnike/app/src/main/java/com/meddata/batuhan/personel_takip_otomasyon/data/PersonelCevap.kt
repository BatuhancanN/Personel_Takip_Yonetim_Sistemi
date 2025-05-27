package com.meddata.batuhan.personel_takip_otomasyon.data

import com.google.gson.annotations.Expose
import com.google.gson.annotations.SerializedName
import com.meddata.batuhan.personel_takip_otomasyon.classes.Personel
import java.io.Serializable

data class PersonelCevap (

    @SerializedName("personeller")
    @Expose
    var personeller: List<Personel>,

    @SerializedName("success")
    @Expose
    var success: Int,

    ):Serializable {
}