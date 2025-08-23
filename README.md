# rpg_console
*a simple console game to practice c#*

game rpg kết nối database (mysql), 
bối cảnh gồm nhân vật (player) có sức mạnh (máu, sức đánh), tăng level sau khi di map (đánh quái) và làm các nhiệm vụ (quest), 
quest(đánh quái(monster)), monster sinh ngẫu nhiên theo cấp người chơi +-2->3 level,
thưởng sau ải + nhiệm vu: kinh nghiệm (exp), trang bị (weapon).

mở rộng: boss, cửa hàng, đấu trường, bảng xếp hạng level người chơi, phép bổ trợ (spell)...

tóm tắt các chức năng:
1. đăng nhập (chỉ nhập tên người chơi)
2. đánh quái (tấn công/ bỏ chạy (mất kinh nghiệm))
3. người chơi tăng sức mạnh dựa vào level
4. trang bị vũ khí tăng sức mạnh (hiện tại: kiếm các loại, khiên)
5. xem thông tin người chơi (lịch sử đánh quái, level, sức mạnh...)
6. thoát trò chơi
7. ...

luồng chính:
1.đăng nhập ->  2.  nhập các mục chức năng
(hoặc thoát)    2.1 đánh quái -> 2.1.1 đánh (thành công tăng kinh nghiệm)
                              -> 2.1.2 bỏ (mất kinh nghiệm)
                              (hiện tại: hồi đầy máu sau đánh)
                2.2 nhiệm vụ (yêu cầu đánh số lượng quái...)
                2.3 hành trang -> 2.2.1 trang bị vũ khí
                2.4 thông tin player (level, số quái đã tiêu diệt, sức mạnh hiện tại)
                                                                                     -> 3. thoát (quay lại 1.đăng nhập)
