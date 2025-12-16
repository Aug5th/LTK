# Võ Lâm Tu Tiên – MVP

**Võ Lâm Tu Tiên** là một game ARPG 3D lấy cảm hứng từ Võ Lâm, nhưng đặt trong **thế giới Tu Tiên**: khởi đầu từ một kẻ mới bước chân vào con đường tu hành, luyện nội công, đột phá cảnh giới, săn boss để đoạt công pháp và pháp bảo mạnh hơn.

Tài liệu này giới thiệu về **bản MVP** (Minimum Viable Product) – phiên bản khung gameplay để thử nghiệm vòng lặp cốt lõi, cảm giác auto-farm và hệ thống nội công/cảnh giới.

---

## 1. Thông tin chung

- **Thể loại:** Top-down 3D ARPG, auto-farm, Tu Tiên  
- **Engine:** Unity (3D)  
- **Nền tảng:** PC (Windows)  
- **Chế độ:** Singleplayer (offline cho MVP)

### Mục tiêu bản MVP

MVP tập trung vào:

- Di chuyển & chiến đấu cơ bản (click chuột phải để di chuyển, auto-farm quái thường).
- Hệ thống **Nội Công & Cảnh Giới** (thay cho level nhân vật).
- 2 bộ nội công chính:
  - `Sơ Tâm Công Quyết` – nội công khởi đầu, thiên về **hồi phục, cắm auto ổn định**.
  - `Thanh Vân Kiếm Quyết` – nội công Kiếm Tu, thiên về **sát thương & stack “Kiếm Hồn”**.
- 1 bãi quái luyện khí + 1 bí cảnh với Boss Luyện Khí:
  - Bãi quái: farm Kinh Nghiệm Nội Công (KNNC) & đồ cơ bản.
  - Boss: rơi **nội công cao cấp** (`Thanh Vân Kiếm Quyết`).
- Cảm giác vòng lặp:
  > auto-farm → tu luyện nội công → đột phá cảnh giới → săn boss → mở nội công mới.

---

## 2. Core Gameplay Loop (Vòng lặp chính)

Vòng lặp 5–20 phút trong MVP:

1. **Khởi đầu tại Thôn Khởi Tu**
   - Nhân vật bắt đầu với nội công:
     - `Sơ Tâm Công Quyết` – Tầng 1 (Luyện Khí 1).
   - Được hướng dẫn:
     - Di chuyển bằng chuột phải.
     - Tấn công quái cơ bản.

2. **Ra Bãi Quái Luyện Khí**
   - Click chuột phải để di chuyển đến khu vực quái.
   - Có thể bật **auto-farm**:
     - Nhân vật tự tìm quái thường gần nhất và tấn công.

3. **Farm quái → tăng Kinh Nghiệm Nội Công (KNNC)**
   - Mỗi quái chết:
     - Cộng **KNNC** cho nội công đang luyện.
     - Nhận vàng, linh thạch, item cơ bản.
   - Khi KNNC đủ:
     - Nội công tăng **Lv**, đạt mốc → tăng **Tầng** → tăng **Cảnh Giới**.
   - Với `Sơ Tâm Công Quyết`: từ **Luyện Khí 1 → Luyện Khí 9**.

4. **Tu luyện đến Luyện Khí 9**
   - `Sơ Tâm Công Quyết` có **9 Tầng**, ứng với Luyện Khí 1–9.
   - Mỗi Tầng:
     - Tăng chỉ số cơ bản (HP/ATK/MP theo design).
     - Kỹ năng nội công `Tĩnh Nguyên` hồi máu/mana **mỗi 5s** mạnh hơn.
   - Khi đạt Tầng 9:
     - Người chơi “chạm trần” với nội công cơ bản, cần **công pháp mới** để tiến xa hơn.

5. **Mở Bí Cảnh Luyện Khí – Đối đầu Boss**
   - Đạt Luyện Khí 9:
     - Mở quyền vào **Bí Cảnh Luyện Khí**, có boss `Hổ Yêu Luyện Khí`.
   - Đánh bại Boss:
     - Nhận nhiều KNNC, vàng, trang bị xanh.
     - Đặc biệt: nhận **nội công mới** `Thanh Vân Kiếm Quyết` (bảo đảm rơi lần đầu).

6. **Đổi sang Thanh Vân Kiếm Quyết – Chu kỳ tu luyện mới**
   - Người chơi chọn **luyện nội công Thanh Vân Kiếm Quyết**:
     - Stats mỗi tầng mạnh hơn nhiều.
     - Tầng cao tương đương **Trúc Cơ** (phục vụ unlock nội dung về sau).
     - Có kỹ năng stack `Kiếm Hồn` giúp **dồn sát thương lên boss**.

---

## 3. Hệ thống Nội Công & Cảnh Giới

### 3.1. Thay thế Level bằng Nội Công

- Game **không dùng level nhân vật** theo kiểu truyền thống.
- Thay vào đó:
  - Đánh quái → nhận **Kinh Nghiệm Nội Công (KNNC)**.
  - KNNC → tăng **Lv Nội Công** → đạt mốc → tăng **Tầng** → tăng **Cảnh Giới**.

### 3.2. Tầng & Cảnh Giới (MVP)

- Mỗi nội công trong MVP có **9 Tầng**.
- Mapping đơn giản:
  - Tầng 1–9 = **Luyện Khí 1–9**.
- Với nội công mạnh (`Thanh Vân Kiếm Quyết`), các Tầng cao (8–9) có thể được xem như **Trúc Cơ 1–2** khi check điều kiện cho map/trang bị ở các bản sau.

---

## 4. Nội Công trong MVP

### 4.1. Sơ Tâm Công Quyết

- **Loại:** Nội công cơ bản (Thường).  
- **Vai trò:** Dùng để làm quen game: **dễ tu, hồi phục tốt, cắm auto an toàn**.  
- **Tầng:** 1–9, tương ứng Luyện Khí 1–9.

#### Kỹ năng nội công: Tĩnh Nguyên (Passive – Auto regen)

- Hoạt động:
  - Cứ **mỗi 5 giây**, tự động hồi một lượng **HP** và **MP cố định** theo Tầng.
  - Luôn hồi, kể cả khi:
    - Không tấn công.
    - Không bị tấn công.
  - Không hồi quá MaxHP/MaxMP (không overheal).

- Ví dụ giá trị tick (tham khảo):

| Tầng | HP/5s | MP/5s |
|------|-------|-------|
| 1    | 6     | 3     |
| 3    | 12    | 7     |
| 5    | 20    | 11    |
| 9    | 45    | 22    |

> Bảng chi tiết đầy đủ (1–9) nằm trong tài liệu design/Excel.

#### Upgrade Tầng 9: Hồi Nguyên Trấn

Khi `Sơ Tâm Công Quyết` đạt Tầng 9:

- `Tĩnh Nguyên` được tăng sức:
  - HP/MP mỗi tick (~5s) tăng thêm khoảng **20%**.
- Thêm **hiệu ứng burst định kỳ** (hoàn toàn auto):
  - Mỗi ~90 giây:
    - Hồi ngay ~**12% MaxHP** + **15% MaxMP**.
    - Trong 3 giây kế tiếp:
      - +4% MaxHP/s.
      - +3% MaxMP/s.
- Tạo cảm giác: **đạt Tầng 9 = bước vào trạng thái hồi phục rất bền cho auto-farm**.

---

### 4.2. Thanh Vân Kiếm Quyết

- **Loại:** Nội công hiếm (Kiếm Tu).  
- **Nguồn:** Rơi từ Boss Bí Cảnh Luyện Khí.  
- **Vai trò:** Tăng mạnh DPS, nhất là đánh boss/elite.

#### Kỹ năng nội công: Kiếm Hồn (Passive – Stack trên mục tiêu)

- Hoạt động cơ bản:
  - Mỗi lần đòn đánh/skill trúng kẻ địch:
    - Thêm 1 **stack Kiếm Hồn** (tối đa 3 stack/mục tiêu).
    - Stack tồn tại ~6s (đặt mới → refresh thời gian).
  - Mỗi stack:
    - Làm mục tiêu **nhận thêm % sát thương từ nhân vật**.
  - Khi mục tiêu đạt 3 stack:
    - Stack tự động **kích nổ**:
      - Gây thêm một lần damage lớn dựa trên ATK (explosion).
      - Gây một lượng damage nhỏ AOE quanh mục tiêu.

- Các thông số (per stack %, hệ số explosion…) **tăng theo Tầng**, giúp:
  - Ở Tầng cao, dồn sát thương vào boss cảm giác rất rõ.

#### Upgrade Tầng 9: Thiên Vân Kiếm Hồn

Khi `Thanh Vân Kiếm Quyết` đạt Tầng 9:

- Kiếm Hồn được tăng sức:
  - % sát thương mỗi stack tăng thêm (ví dụ Tier9 ~10%/stack).
  - Explosion:
    - Damage chính tăng (nhân ×1.4).
    - Thêm phần **true damage** (bỏ qua phòng thủ) theo % explosion.
    - AOE damage rộng và mạnh hơn.
- Có thể bổ sung:
  - Chance tạo thêm “Kiếm Vân” bay sang mục tiêu phụ → gây thêm damage + đặt stack.
  - Buff ngắn (6s) sau mỗi lần explosion (tăng Crit/Attack Speed).

> MVP có thể bắt đầu với bản đơn giản (stack → nổ → damage lớn hơn), rồi dần thêm hiệu ứng phụ khi đã cân bằng xong.

---

## 5. Auto-Farm (MVP)

- Toggle Auto (ví dụ phím `T` hoặc nút UI).
- Khi **Auto ON**:
  - Nhân vật:
    - Tự tìm quái thường gần nhất.
    - Tự di chuyển lại gần (dựa theo hệ thống click-to-move).
    - Tự tấn công cho đến khi quái chết.
    - Lặp lại.

Kết hợp với nội công:

- `Sơ Tâm Công Quyết`:
  - Tĩnh Nguyên → **tự hồi HP/MP** giúp cắm auto lâu, ít phải dùng bình.
- `Thanh Vân Kiếm Quyết`:
  - Kiếm Hồn → **stack & nổ auto** → tăng DPS khi đánh lâu trên 1 mục tiêu.

---

## 6. Điều khiển cơ bản (MVP)

- **Chuột phải:**
  - Click lên mặt đất → nhân vật di chuyển đến vị trí đó.
- **Tấn công:**
  - MVP: có thể tự động tấn công quái trong tầm khi auto-farm BẬT, hoặc click chuột lên quái (tùy implement).
- **Auto-farm:**
  - Phím tắt (dự kiến): `T` → bật/tắt auto.

---

## 7. Trạng thái hiện tại & hướng phát triển

Hiện tại (định hướng MVP):

- Có:
  - Thiết kế hệ Nội Công & Cảnh Giới (Sơ Tâm Công Quyết, Thanh Vân Kiếm Quyết).
  - Thiết kế kỹ năng nội công: `Tĩnh Nguyên` (regen), `Kiếm Hồn` (stack).
  - Thiết kế bãi quái luyện khí + Boss Bí Cảnh Luyện Khí.
  - Code cơ bản di chuyển bằng chuột phải.

- Sắp/đang làm:
  - Auto-farm quái.
  - Combat đơn giản (hit, nhận damage, chết).
  - UI: thanh HP/MP, Cảnh Giới, chọn nội công.
  - Asset 2D cơ bản cho Kiếm Tu & quái.

Sau MVP:

- Thêm nội công mới (thuộc tính Hỏa/Thủy/Thể…).
- Mở rộng cảnh giới (Trúc Cơ đầy đủ, Kim Đan…).
- Thêm map, bí cảnh, hệ trang bị/pháp bảo chi tiết.
- Cơ chế tĩnh tọa, luyện đan, độ kiếp, v.v.

---
