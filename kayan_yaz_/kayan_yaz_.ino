/*
**********************************************************************
* Kayan Başlık 
* Atari Kabini Tabelası İçin Kayan Yazı Şeridi
* Kodlama:Enes BİBER
**********************************************************************
*/

#include <Adafruit_NeoPixel.h>

#define PIN 4                                  // Led şerit Data pin giriş


int numOfLeds = 72;                           //Şerit üzerindeki Led Sayısı
int minBrightness = 80;                       // parlaklık en düşük (0-255)
int maxBrightness = 255;                      // parlaklık en yüksek (0-255)
int walkingLedsDelay = 100;                   // yürüyen ledler arasındaki bekleme süresi (ms)
int flashDelay = 150;                          //Flash efekti süresi
int numOfFlashLoops = 10;                     // Flash efekt  tekrarı
int numOfPulseLoops = 10;                     //...
int pulseDelay = 0;                           // ..
int RenkSec = 0;  
int TEMPKod=0;    


Adafruit_NeoPixel strip = Adafruit_NeoPixel(numOfLeds, PIN);


void setup() {
    Serial.begin(9600);
  strip.begin();

}


void loop() {
  walkingLeds();   // yürüyen ledleri gör 
  RenkSec=random(3);  
  while(RenkSec==TEMPKod){
  RenkSec=random(3);    
  }
 TEMPKod=RenkSec;
  flashLeds(RenkSec);   // flash animasyon
  Serial.println("Renk kodu: " +(String)RenkSec);

  for (int i = 0; i < numOfPulseLoops; i++)   
    pulseBrightness(RenkSec);



}


void walkingLeds() {
  //setLedColorsToZero();
  strip.setBrightness(maxBrightness);
  strip.show();
 
  for (int x = numOfLeds; x > 0; x--) { 

    strip.setPixelColor(x, strip.Color(0,0,0));
    strip.show();
    delay(walkingLedsDelay );
    strip.setPixelColor(x, 0);
    strip.show();
  }  
}

void flashLeds(int Renk) {
  
  setLedColors(Renk);

  for (int i = 0; i < numOfFlashLoops; i++) {
    strip.setBrightness(maxBrightness);
    strip.show();
    delay(flashDelay );

    strip.setBrightness(minBrightness);
    strip.show();
    delay(flashDelay );
  }
}

void pulseBrightness(int Renk) {
  setLedColors(Renk);

  for (int i = minBrightness; i < maxBrightness; i++) {
    strip.setBrightness(i);
    strip.show();
    delay(pulseDelay);
  }

  for (int i = maxBrightness; i > minBrightness; i--) {
    strip.setBrightness(i);
    strip.show();
    delay(pulseDelay);
  }
}

void setLedColors(int renk) {
  if (renk == 0) {
  // Kirmizi Ledler
   for (int x = 0; x < numOfLeds; x++)
  strip.setPixelColor(x, strip.Color(255,0,0));

}
else if (renk == 1) { 
  //Yeşil Ledler
   for (int x = 0; x < numOfLeds; x++)
  strip.setPixelColor(x, strip.Color(0,255,0));

}
else {
  // Mavi Ledler
   for (int x = 0; x < numOfLeds; x++)
  strip.setPixelColor(x, strip.Color(0,0,255));

}
 
}

void setLedColorsToZero() {
  for (int x = 0; x < numOfLeds; x++)
    strip.setPixelColor(x, strip.Color(0, 0, 0));
}
