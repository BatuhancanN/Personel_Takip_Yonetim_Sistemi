package com.meddata.batuhan.personel_takip_otomasyon.data

import retrofit2.Call
import retrofit2.http.Field
import retrofit2.http.FormUrlEncoded
import retrofit2.http.POST

interface PersonelDAOInterface {

    @POST("my/select_personel.php")
    @FormUrlEncoded
    fun kisiyiBul(@Field("personel_id") personel_id: Int): Call<PersonelCevap>

    @POST("my/insert_hareket.php")
    @FormUrlEncoded
    fun hareketEkle(
        @Field("h_personel_id") h_personel_id: Int,
        @Field("h_durumu") h_durumu: Int,
        @Field("h_tarih") h_tarih: String
    ): Call<CRUDcevap>

}