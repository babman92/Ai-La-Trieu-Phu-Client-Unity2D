using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class Command
    {
        public const string ERROR = "error";
        public const string STATUS = "status";
        //--------------------------------------------------------
        public const string client_connect = "connection";
        public const string client_disconnect = "disconnect";
        public const string client_message = "client.message";
        //---------------------client custom----------------------
        public const string client_login = "client.login";
        public const string client_signup = "client.signup";
        public const string client_choose_game = "client.choose.game";
        public const string client_choose_room = "client.choose.room";
        public const string client_invite_friend = "client.invite.friend";
        public const string client_chat = "client.chat.message";

        public const string client_get_question = "client.get.question";
        public const string client_answer = "client.answer";
        public const string client_get_listroom = "client.get.list.room";
        public const string client_signup_name = "client.signup.name";
        public const string client_get_record = "client.get.record";
        public const string client_save_record = "client.save.record";
        public const string client_reconnect = "client.reconnect";
        //-------------------------
        public const string client_answer_to_room = "client.answer.to.room";
        public const string client_ready_to_room = "client.ready.to.room";
        public const string client_time_out_to_room = "client.time.out.to.room";
        public const string client_leave_room = "client.leave.room";
        //---------------------server custom------------------
        public const string server_send_client_join_room = "server.send.client.join.room";
        public const string server_send_question = "server.send.question";
        public const string server_comfirm_answer = "server.confirm.answer";
        public const string server_send_listroom = "server.send.listroom";
        public const string server_send_confirm_signup = "server.send.confirm.signup";
        public const string server_send_record = "server.send.record";
        public const string server_confirm_save_record = "server.confirm.save.record";
        public const string server_confirm_login = "server.confirm.login";
        public const string server_send_reconnect = "server_send_reconnect";
        //-------------------------
        public const string server_to_room_send_question = "server.to.room.send.question";
        public const string server_to_room_confirm_answer = "server.to.room.confirm.answer";
        public const string server_to_room_confirm_ready = "server.to.room.confirm.ready";
        public const string server_to_room_send_finish_match = "server.to.room.send.finish.match";
        public const string server_to_room_send_finish_question = "server.to.room.send.finish.question";
        public const string server_to_room_start_game = "server.to.room.start.game";
        public const string server_to_room_client_leave = "server.to.room.client.leave";
    }
}
