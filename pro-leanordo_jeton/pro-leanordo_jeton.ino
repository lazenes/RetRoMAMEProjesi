/*
   Enes BİBER Bozuk Para Onay Sistemi
   Tarih    : 15.01.2022
   Versiyon : 1v
   Kodlama  : Enes BİBER
   Web Site : WWW.EnesBiBER.COM.TR
*/
#include "Keyboard.h"
volatile int para = 0;
boolean giris = false;
unsigned long bakiye;
void setup() {
   Keyboard.begin();
  Serial.begin(9600);
  Serial.setTimeout(1000);
  //digitalPinToInterrupt(3) yani 3 nolu dijital Portu kullandık
  attachInterrupt(digitalPinToInterrupt(3), paraSayma, RISING); 
}
void loop() {
  if (giris) {
    giris = false;
    //Para Sensörü 2 Defa Sisteme Sinyal Attığı için 2 Okumayı tek'e Düşürüyoruz
    if ( para == 1) {
      Serial.println("Jeton_Girdi"); 
     //Jeton Kanalından Sinyal Gelir ise  0.4 saniye  boyunca 3 tuşuna basar
Keyboard.press('3');
 delay(400);
Keyboard.release('3');
      para = 0;
    }
    bakiye += 1;
    //Serial.println("Bakiye : "+(String)bakiye);
  }
while(Serial.available() > 0 ){
    String str = Serial.readString();
    if(str.indexOf("jeton") > -1){
      Serial.println("Servis_Jetonu_Girdi");
      //Seri Port'a Komut Gelir ise  0.4 saniye  boyunca 3 tuşuna basar
   Keyboard.press('3');
 delay(400);
Keyboard.release('3');
      }else{
      Serial.println("Gecersiz Servis Komutu");
    }
  }
}
void paraSayma() {
  para++ ;
  giris = true;
}
