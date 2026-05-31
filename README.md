## Telif Hakkı ve Lisans

© 2026 Salih İlhan. Tüm hakları saklıdır.

Bu proje MIT Lisansı altında lisanslanmıştır. Daha fazla bilgi için LICENSE dosyasını okuyun.

# 🏃‍♂️ Project RUN (Vertical Slice)
<img width="1440" height="2560" alt="Loading_" src="https://github.com/user-attachments/assets/bf3b1743-b7e3-467f-9041-871002466228" />

Project RUN is a mobile hybrid-casual runner developed in **Unity 6000.3.5f2**. Unlike traditional endless runners that rely on an "all-or-nothing" high-score model, this project introduces a **Level-Based Progression System** combined with RPG-style **Meta-Progression** to reduce player frustration and increase long-term retention.

> 💡 **Designer's Note:** This repository contains a polished vertical slice. Core running gameplay, hit/damage mechanics, main menu navigation, and the character stat upgrade loop are fully implemented.

---

## 🚀 Key Features & Design Philosophy

### 🛠️ Core Gameplay Mechanics
* **Level-Based Structure:** Replaces the exhausting endless loop with definitive milestones, giving players a clear sense of progression and short-term achievement.
* **Health & Damage System:** Hitting an obstacle reduces the player's health pool (%100 \rightarrow 75%) instead of causing an instant game-over. This shifts player motivation from pure twitch-reflexes to strategic resource management.
* **Pacing (Kishōtenketsu Structure):** Levels are structured to introduce a new obstacle type, scale its difficulty, and then offer a low-stress "breather level" packed with coins before ramping up again.

### 🪙 Meta-Progression & Game Economy
A balanced **Taps & Sinks** ecosystem designed around a modular character upgrade screen:
* **Speed:** Changes the risk/reward ratio by increasing velocity vs. reaction windows.
* **Resistance:** Directly increases the player's health pool to survive more collisions.
* **Explorer:** Multiplies the detection/spawn rate of rare map components.
* **Lucky:** Increases regular coin and piece inflow during runs.

---

## 🛠️ Technical Stack & Implementation

* **Game Engine:** Unity 6000.3.5f2 (Safe Mode Verified)
* **Render Pipeline:** Universal Render Pipeline (URP 3D)
* **UI/UX Architecture:** 
  * High-contrast, "juicy" UI design elements mapped to adaptive layout groups.
  * Native Unity UI components integrated with scalable sprite atlases for draw-call optimization.
  * Monetization-ready screen hooks (Rewarded Ad simulations for stat multipliers, Special Offers, and Dual-Currency Shop architecture).

---

## 📸 Screenshots & UI Previews

### 1. Main Menu & Level Progression
*Visual onboarding and progression tracking.*
![Main Menu]
<img width="1440" height="2560" alt="Artboard 1" src="https://github.com/user-attachments/assets/0e8df537-c35e-44f3-89c1-5ef880e4f8c5" />


### 2. Character Upgrades & Stats Panel
*The core meta-game loop showing individual stat upgrades via Gold/Diamonds/Ads.*
![Character Upgrades]
<img width="1440" height="2560" alt="Çalışma yüzeyi 2" src="https://github.com/user-attachments/assets/00d6bb0e-6cf5-49f5-840b-181f040cb32c" />
<img width="1440" height="2560" alt="CHARACTER_FEATURES" src="https://github.com/user-attachments/assets/2f25a8ec-c322-4f2b-85f6-f80e5ccb3108" />

### 3. Gameplay & Pause States
*Real-time runner gameplay featuring low-poly assets and the integrated health state.*
![Gameplay]
<img width="547" height="966" alt="Ekran görüntüsü 2026-05-11 231129" src="https://github.com/user-attachments/assets/4656e4a5-1552-4fd4-adc4-34b896a6823f" />

![Pause Menu]
<img width="543" height="964" alt="Ekran görüntüsü 2026-05-11 231257" src="https://github.com/user-attachments/assets/d35a1ce4-526e-4e4b-a937-40f2c469ab7b" />

---

![Uploading Çalışma yüzeyi 2.png…]()

## 🧭 Unimplemented Vision (Future Roadmap)
Due to a solo-developer scoping crunch (burnout phase), the following high-fidelity mechanics currently exist in the design layer but are not fully hooked into the core loop:
1. **Dynamic Roadside Shops:** Designing side-lanes where a player can steer completely to the right/left to trigger a slow-motion shop window, allowing them to purchase temporary power-ups (Shielding, Healing, Acceleration) mid-run.
2. **Component Integration:** The logic for discovering and combining map parts to unlock specialized boosts.

---

## 📄 License
This project is for portfolio and demonstration purposes only. All design rights, UI assets, and code structures belong to the repository owner.
