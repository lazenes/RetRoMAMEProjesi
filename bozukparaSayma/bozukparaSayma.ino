/*
   Enes BİBER Bozuk Para Onay Sistemi
   Tarih    : 17.11.2021
   Versiyon : 0.1b
   Kodlama  : Enes BİBER
   Web Site : WWW.EnesBiBER.COM.TR
*/


volatile int para = 0;
boolean giris = false;

unsigned long bakiye;

 

void setup() {
    pinMode(8,OUTPUT);
  Serial.begin(9600);
  attachInterrupt(digitalPinToInterrupt(2), paraSayma, RISING);
  
  delay(1000);
  
  
}

void loop() {
  if (giris) {
    giris = false;
    //Para Sensörü 2 Defa Sisteme Sinyal Attığı için 2 Okumayı tek'e Düşürüyoruz
    if ( para == 1) {
      Serial.println("Jeton_Girdi");      
   RoleAcKapat();
      para = 0;
    }
    bakiye += 1;
    //Serial.println("Bakiye : "+(String)bakiye);

  }
}

//GirdiSayma Fonksyonu
void paraSayma() {

  para++ ;
  giris = true;
}

//GirdiSayma Fonksyonu
void RoleAcKapat() {
   delay(8); 
 digitalWrite(8,HIGH);
//  Serial.println("Role Açık"); 
 delay(8); 
   digitalWrite(8,LOW);
  //Serial.println("Role Kapalı"); 
}
