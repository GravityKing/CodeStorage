package com.example.myplugin;

import androidx.activity.result.ActivityResult;
import androidx.activity.result.ActivityResultCallback;
import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import android.Manifest;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.location.Location;
import android.media.ExifInterface;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.provider.Settings;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;

public class GetGeoData extends AppCompatActivity{

    private float latitude;
    private float longitude;
    static final int PERMISSIONS_WRITE_LOCATION = 0x00000001;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    

        ActivityCompat.requestPermissions(this,new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE,
                                                                   Manifest.permission.ACCESS_MEDIA_LOCATION,
                                                                   Manifest.permission.READ_EXTERNAL_STORAGE,
                                                                   Manifest.permission.MANAGE_EXTERNAL_STORAGE},PERMISSIONS_WRITE_LOCATION);



        int permission = ContextCompat.checkSelfPermission(this,
                Manifest.permission.WRITE_EXTERNAL_STORAGE);

        int permission2 = ContextCompat.checkSelfPermission(this,
                Manifest.permission.ACCESS_MEDIA_LOCATION);

        int permission3 = ContextCompat.checkSelfPermission(this,
                Manifest.permission.READ_EXTERNAL_STORAGE);

        int permission4 = ContextCompat.checkSelfPermission(this,
                Manifest.permission.MANAGE_EXTERNAL_STORAGE);

        System.out.println("WRITE_EXTERNAL_STORAGE: " + permission);
        System.out.println("ACCESS_MEDIA_LOCATION: " + permission2);
        System.out.println("READ_EXTERNAL_STORAGE: " + permission3);
        System.out.println("MANAGE_EXTERNAL_STORAGE: " + permission4);
        System.out.println("checkPermission: " + checkPermission());
        try {
            Thread.sleep(2000);
        }
        catch (InterruptedException e){

        }

        String path = "/storage/emulated/0/DCIM/새폴더/main3.jpg";
        String latitudeRef = "35/1,38/1,19881240/1000000";
        String longitudeRef = "126/1,26/1,18666600/1000000";
        setImageMetaRef(path,latitudeRef,longitudeRef);

        try {
            Thread.sleep(2000);
        }
        catch (InterruptedException e){

        }


        String[] result = getImageMetaRef(path);
        System.out.println("result: " + result[0]);
        System.out.println("result: " + result[1]);

    }

    // 위도 경도 가져오기
    public String[] getImageMeta(String picturePath){

        String[] metaArr = new String[2];

        try {
            ExifInterface exif = new ExifInterface(picturePath);
            metaArr = getMeta(exif);
        } catch (IOException e) {
            e.printStackTrace();
        }

        return metaArr;
    }

    // 위도 경도 Ref 가져오기
    public String[] getImageMetaRef(String picturePath){
        String[] metaArr = new String[2];

        try {
            ExifInterface exif = new ExifInterface(picturePath);
            String attrLATITUDE = exif.getAttribute(ExifInterface.TAG_GPS_LATITUDE);
            String attrLONGITUDE = exif.getAttribute(ExifInterface.TAG_GPS_LONGITUDE);
            metaArr[0] = attrLATITUDE;
            metaArr[1] = attrLONGITUDE;
        } catch (IOException e) {
            e.printStackTrace();
        }

        return metaArr;
    }

    // 위도 경도 Ref 수정
    public  String[] setImageMetaRef(String picturePath, String latitudeREF, String longitudeREF){
        String[] metaArr = new String[2];

        try {
            ExifInterface exif = new ExifInterface(picturePath);
            double doubleLat = convertToDegree(latitudeREF);
            double doubleLon = convertToDegree(longitudeREF);
            System.out.println("doubleLat: " + doubleLat );
            System.out.println("doubleLon: " + doubleLon );

            double alat = Math.abs(doubleLat);
            double alon = Math.abs(doubleLon);
            System.out.println("alat: " + alat );
            System.out.println("alon: " + alon );
            String dms = Location.convert(alat, Location.FORMAT_SECONDS);
            System.out.println("dms: " + dms );
            String[] splits = dms.split(":");
            String[] secnds = (splits[2]).split("\\.");
            String seconds;

            if(secnds.length==0)
            {
                seconds = splits[2];
            }
            else
            {
                seconds = secnds[0];
            }
            System.out.println("seconds: " + seconds );
            String latitudeStr = splits[0] + "/1," + splits[1] + "/1," + seconds + "/1";
            System.out.println("latitudeStr: " + latitudeStr );


            exif.setAttribute(ExifInterface.TAG_GPS_LATITUDE, latitudeStr);
            exif.setAttribute(ExifInterface.TAG_GPS_LATITUDE_REF, doubleLat>0?"N":"S");

            dms = Location.convert(alon, Location.FORMAT_SECONDS);
            splits = dms.split(":");
            secnds = (splits[2]).split("\\.");

            if(secnds.length==0)
            {
                seconds = splits[2];
            }
            else
            {
                seconds = secnds[0];
            }
            String longitudeStr = splits[0] + "/1," + splits[1] + "/1," + seconds + "/1";


            exif.setAttribute(ExifInterface.TAG_GPS_LONGITUDE, longitudeStr);
            exif.setAttribute(ExifInterface.TAG_GPS_LONGITUDE_REF, doubleLon>0?"E":"W");

            exif.saveAttributes();

        }
        catch (IOException e){
            System.out.println(e.getLocalizedMessage());
        }
        return metaArr;
    }

    private float convertToDegree(String stringDMS) {
        Float result = null;
        String [] DMS = stringDMS.split(",",3);

        String[] stringD = DMS[0].split("/",2);
        Double D0 = Double.valueOf(stringD[0]);
        Double D1 = Double.valueOf(stringD[1]);
        Double FloatD = D0/D1;

        String[] stringM = DMS[1].split("/",2);
        Double M0 = Double.valueOf(stringM[0]);
        Double M1 = Double.valueOf(stringM[1]);
        Double FloatM = M0/M1;

        String[] stringS = DMS[2].split("/",2);
        Double S0 = Double.valueOf(stringS[0]);
        Double S1 = Double.valueOf(stringS[1]);
        Double FloatS = S0/S1;

        result = (float) (FloatD + (FloatM / 60) + (FloatS / 3600));

        return result;
    }

    public String[] getMeta(ExifInterface exif) {
        String[] result = new String[2];
        String attrLATITUDE = exif.getAttribute(ExifInterface.TAG_GPS_LATITUDE);
        String attrLONGITUDE = exif.getAttribute(ExifInterface.TAG_GPS_LONGITUDE);
        longitude = convertToDegree(attrLONGITUDE);
        latitude = convertToDegree(attrLATITUDE);
        result[0] = String.valueOf(longitude);
        result[1] = String.valueOf(latitude);
        return result;

    }
}