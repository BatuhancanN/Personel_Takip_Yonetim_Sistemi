package com.meddata.batuhan.personel_takip_otomasyon.data

import com.google.gson.annotations.Expose
import com.google.gson.annotations.SerializedName
import java.io.Serializable

data class CRUDcevap (

    @SerializedName("success")
    @Expose
    var success: Int,

    @SerializedName("message")
    @Expose
    var message: String

): Serializable {
}