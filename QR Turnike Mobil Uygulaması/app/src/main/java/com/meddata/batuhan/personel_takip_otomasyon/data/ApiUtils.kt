package com.meddata.batuhan.personel_takip_otomasyon.data

import android.content.Context
import android.content.SharedPreferences

class ApiUtils {

    companion object {

        fun getPersonelDAOInterface(context: Context): PersonelDAOInterface {

            val sharedPreferences: SharedPreferences = context.getSharedPreferences("BTUServerCon", Context.MODE_PRIVATE)
            var baseUrl = sharedPreferences.getString("server", "") ?: ""

            val isValidUrl = baseUrl.isNotEmpty() && (baseUrl.startsWith("http://") || baseUrl.startsWith("https://"))

            if (!isValidUrl || baseUrl == "http://" || baseUrl == "https://") {
                val editor = sharedPreferences.edit()
                editor.remove("server")
                editor.apply()

                baseUrl = "http://10.30.172.250/" //ilk kurulumda gelen sabit ip
            }

            return RetrofitClient.getClient(baseUrl).create(PersonelDAOInterface::class.java)
        }
    }

}