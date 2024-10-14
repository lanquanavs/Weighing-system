using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using AWSV2.Services;
using Common.Utility.AJ.Extension;
using Common.Utility.AJ.MobileConfiguration;
using log4net;
using PYun.API;
using PYun.API.Reply;
using PYun.API.Request;
using PYun.Utils;


namespace AWSV2.CloudLogic
{
    public class PYunService : AbstractServiceAPI
    {
        private MobileConfigurationMgr _mobileConfigurationMgr;
        public PYunService(ref MobileConfigurationMgr mobileConfigurationMgr)
        {
            _mobileConfigurationMgr = mobileConfigurationMgr;
        }
        /// <summary>
        /// 提前拦截请求, 若请求被处理则不会触发其他方法回调
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="args"></param>
        /// <param name="reply"></param>
        /// <returns>true - 请求已拦截处理完成; false - 请求未处理</returns>
        public override bool HookRequest(string serviceName, IDictionary<string, string> args,
            PYun.Protocol.Payload.ExecutePayload reply)
        {
            //// 对于SDK尚未封装的接口可在这手动拦截实现
            //if ("service.parking.realtime".Equals(serviceName))
            //{
            //    // 模拟拦截实时车位
            //    IDictionary<string, string> parameters = new Dictionary<string, string>();
            //    parameters["total"] = "1000";
            //    parameters["parking"] = "10";
            //    parameters["available"] = "900";


            //    // 设置返回结果
            //    reply.ResultCode = "1001";
            //    reply.Message = "处理成功";
            //    reply.Parameters = parameters;
            //    // 注意: 拦截处理后返回true表示已经处理过了
            //    return true;

            //}

            //// 订单支付结果通知
            //if ("service.parking.payment.result".Equals(serviceName))
            //{
            //    // 模拟拦截实时车位
            //    IDictionary<string, string> parameters = new Dictionary<string, string>();
            //    parameters["total"] = "1000";
            //    parameters["parking"] = "10";
            //    parameters["available"] = "900";

            //    //获取当前这次停车记录，并修改停车支付状态为 1，电子支付
            //    SQLDataAccess.GetWeighingRecord(reply);

            //    // 设置返回结果
            //    reply.ResultCode = "1001";
            //    reply.Message = "处理成功";
            //    reply.Parameters = parameters;
            //    // 注意: 拦截处理后返回true表示已经处理过了
            //    return true;

            //}


            // 其他未处理返回false则SDK会自动处理
            return false;
        }

        /// <summary>
        /// 查询停车费用
        /// 1001 订单获取成功，业务参数将返回。
        /// 1002 未查询到停车信息。
        /// 1003 月卡车辆，不允许缴费。
        /// 1500 接口处理异常。
        /// 
        /// // 获取订单代码
        /// if (card 不合法 && !plate 合法) {
        ///     return 1400; // 返回参数错误
        /// }
        /// // 1. 检查订单
        /// ParkingOrder parking = 获取停车记录(card, plate) // 根据车牌或停车卡获取停车记录
        /// if (parking 已出场) {
        ///     return 1002; // 无停车记录
        /// }
        /// // 2. 检查是否月卡、次卡等非临停用户
        /// if (parking 非临停) {
        ///     return 1003; // 非临停，返回无需支付停车费
        /// }
        /// // 3. 检查收费情况
        /// if (parking 无需交费) {
        ///     return parking; // 返回停车订单，不含支付单号，包含停车流水
        /// }
        /// // 4. 填充订单数据, 若有优惠，则填充优惠信息；
        /// ParkingBilling billing = 生成停车订单;
        /// parking.parking_order = billing.serial;    // 支付流水...
        /// parking.pay_value = billing.value;  // 实际支付金额
        /// 
        /// return parking; // 返回支付订单。
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //public override APIReply<ParkingBilling> ParkingBilling(ParkingBillingRequest request)
        //{
        //    try
        //    {
        //        var config = _mobileConfigurationMgr.SettingList[SettingNameKey.Main];
        //        var ChargeEnable = config.Settings["ChargeEnable"].Value;
        //        var ChargeWay = config.Settings["ChargeWay"].Value;

        //        //如果不是称重磅秤出场通道的回调，就停止下面的逻辑
        //        if (request.GateId != "002")
        //        {
        //            return APIReply<ParkingBilling>.newBuilder()
        //                .setResultCode(ResultCode.BadRequest)
        //                .setMessage("通道无效")
        //                .Build();
        //        }

        //        //如果是空的车牌号码，就停止下面的逻辑
        //        if (string.IsNullOrEmpty(Globalspace.CurrentOutPlate))
        //        {
        //            return APIReply<ParkingBilling>.newBuilder()
        //                .setResultCode(ResultCode.BadRequest)
        //                .setMessage("车牌号码无效")
        //                .Build();
        //        }
        //        request.Plate = Globalspace.CurrentOutPlate;
        //        //检查沉重记录完整性
        //        var weighingRecord = SQLDataAccess.GetWeighingRecord(request.Plate);

        //        ////2022-10-19 注释。原因：不需要这个验证
        //        //if (!_weighingRecord.IsFinish)
        //        //{
        //        //    return APIReply<ParkingBilling>.newBuilder()
        //        //       .setResultCode(ResultCode.BadRequest)
        //        //       .setMessage("称重记录不完整，禁止通行")
        //        //       .Build();

        //        //}

        //        if (ChargeEnable != "1") {
        //            return APIReply<ParkingBilling>.newBuilder()
        //                 .setResultCode(ResultCode.BadRequest)
        //                 .setMessage("磅秤处不支持缴费")
        //                 .Build();
        //        }

        //        if (config.Settings["EChargeEnable"].Value != "1")
        //        {
        //            return APIReply<ParkingBilling>.newBuilder()
        //                 .setResultCode(ResultCode.BadRequest)
        //                 .setMessage("磅秤处不支持电子缴费")
        //                 .Build();
        //        }

        //        if (ChargeWay != "1")
        //        {
        //            return APIReply<ParkingBilling>.newBuilder()
        //                 .setResultCode(ResultCode.BadRequest)
        //                 .setMessage("不在此处缴费，请前往场地出口处缴费")
        //                 .Build();
        //        }
        //        //2022-12-01 在称重弹出收费框时，数据还没保存到数据库里，这里获取数据库里的金额一直都是0，所以在称重界面里，把算出的金额存到全局变量里，这里再获取出来使用
        //        weighingRecord.Je = AWSV2.Globalspace.CurrentCost;

        //        //查询本次场内消费的费用
        //        decimal feiyong = 0;
        //        decimal.TryParse(weighingRecord.Je, out feiyong);
        //        feiyong = feiyong * 100;//人民币元转换成分
        //        //feiyong = 0;
        //        //计算本次称重需要的费用，并返回给P云 
        //        ParkingBilling payload = new ParkingBilling();
        //        payload.EnterTime = weighingRecord.EntryTime.Value;
        //        payload.Plate = weighingRecord.Ch;
        //        payload.ParkingSerial = weighingRecord.Bh;
        //        payload.ParkingOrder = weighingRecord.Bh;// DateTime.Now.ToString("yyyyMMddHHmmss") + RandomUtil.RandomNumeric(6);
        //        payload.ParkingTime = Utility.TimeSPanBySeconds(weighingRecord.EntryTime.Value);// 90 * 60;  // 单位秒
        //        payload.TotalValue = uint.Parse(feiyong.ToString("F0"));
        //        payload.FreeValue = 0;
        //        payload.PaidValue = 0;
        //        payload.PayValue = uint.Parse(feiyong.ToString("F0"));

        //        //如果是0元，则无需缴费，平台也不会再调用同步函数，所以这里直接修改数据并开闸
        //        if (feiyong <= 0) {
        //            //修改称重的支付状态为电子支付
        //            weighingRecord.IsPay = 1;
        //            weighingRecord.Zfdw = "0元无需缴费";
        //            weighingRecord.Zfje = 0;
        //            weighingRecord.Zfsj = DateTime.Now;
        //            SQLDataAccess.UpdateWeighingRecord(weighingRecord);
        //            //清空车牌，以备下次使用！赋值在SECONDWETGHING,ONECWEIGHING方法中
        //            Globalspace.CurrentOutPlate = string.Empty;
        //            //调用闸机放行车辆
        //            //App.Current.Dispatcher.Invoke((Action)delegate ()
        //            //{
        //                Globalspace.ShellViewModel.CloseDialog();
        //           // });
        //        }

        //        //返回订单数据给P云
        //        return APIReply<ParkingBilling>.newBuilder()
        //            .setResultCode(ResultCode.Success)
        //            .setPayload(payload)
        //            .setMessage("成功")
        //            .Build();

        //    }
        //    catch (Exception e)
        //    {
        //        log.Info($"订单获取接口：错误：{e.Message}");
        //        return APIReply<ParkingBilling>.newBuilder()
        //               .setResultCode(ResultCode.InternalServerError)
        //                .setMessage(e.Message)
        //               .Build();
        //    }

        //}

        /// <summary>
        /// 同步场中支付结果
        /// 1001 接口处理成功，业务参数将返回。
        /// 1403 订单已撤销。
        /// 1500 接口内部处理失败。
        /// 
        /// // 1. 根据parking_order查询订单。
        /// ParkingPayment payment = 根据parking_order查询订单
        /// if (payment == null) {
        ///     return 1002;    // 未找到订单信息
        /// } elseif (payment 已成功处理) {
        ///     return 1001; // 返回处理成功
        /// }
        /// // 2. 根据parking_serial查询停车信息。
        /// ParkingOrder parking = 获取停车记录(parking_serial)；
        /// if (parking 已离场) {
        ///     retuen 1403; // 返回订单撤销，我们会根据这个状态核对订单，如果需要办理退款，我们会办理。
        /// }
        /// // 3. 检查订单是否为最新订单，我们需要保证用户支付的最新订单，才能保证支付金额足够
        /// if (payment 非本次停车最新订单) {
        ///     return 1403; // 返回订单撤销，我们会根据这个状态核对订单，如果需要办理退款，我们会办理。
        /// }
        /// // 4. 更新订单状态并更新总支付金额。
        /// update(parking);
        /// 
        /// return 1001; // 返回处理成功
        /// </summary>
        //public override APIReply<object> ParkingPayment(ParkingPaymentRequest request)
        //{
        //    try
        //    {
        //        var config = _mobileConfigurationMgr.SettingList[SettingNameKey.Main];
        //        var ChargeWay = config.Settings["ChargeWay"].Value;
        //        if (request.ParkingSerial == null)
        //        {
        //            return APIReply<object>.newBuilder()
        //                .setResultCode(ResultCode.BadRequest)
        //                .setMessage("参数无效")
        //                .Build();
        //        }
        //        //根据订单号获取本次收费车辆的信息，并且修改收费状态 ispay=1 电子支付
        //        var record = SQLDataAccess.GetWeighingRecordByBh(request.ParkingOrder);

        //        //如果没有找到订单，说明平台传递数据有问题
        //        if (record == null)
        //        {
        //            return APIReply<object>.newBuilder()
        //               .setResultCode(ResultCode.Empty)
        //               .setMessage("未找到称重订单信息")
        //               .Build();
        //        }
               

        //        //修改称重的支付状态为电子支付
        //        record.IsPay = 1;
        //        record.Zfdw = request.PayOrigin.ToString();
        //        record.Zfje = request.Value;
        //        record.Zfsj = request.PayTime;
        //        record.Zflsh = request.PaySerial;
        //        record.Zfdzsm = request.PayOriginDesc;
        //        SQLDataAccess.UpdateWeighingRecord(record);
        //        //清空车牌，以备下次使用！赋值在SECONDWETGHING,ONECWEIGHING方法中
        //        Globalspace.CurrentOutPlate = string.Empty;
        //        //调用闸机放行车辆
        //        //App.Current.Dispatcher.Invoke((Action)delegate ()
        //        //{
        //            Globalspace.ShellViewModel.CloseDialog();
        //        //});

        //        //Globalspace.ShellViewModel.OpenBarrier();
        //        return APIReply<object>.newBuilder()
        //            .setResultCode(ResultCode.Success)
        //            .Build();
        //    }
        //    catch (Exception e)
        //    {
        //        log.Info($"支付同步接口：错误：{e.Message}");
        //        return APIReply<object>.newBuilder()
        //            .setResultCode(ResultCode.InternalServerError)
        //            .setMessage(e.Message)
        //            .Build();
        //    }
        //}

        /// <summary>
        /// 无牌车直接入场请求
        /// 1001 接口处理成功，业务参数将返回。
        /// 1002 未检测到车辆
        /// 1403 短时间重复入场。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<ParkingDetail> ParkingDirectEnter(ParkingDirectEnterRequest request)
        {
            throw new NotImplementedException();
            //if (string.IsNullOrWhiteSpace(request.Passport))
            //{
            //    return APIReply<ParkingDetail>.newBuilder()
            //        .setResultCode(ResultCode.BadRequest)
            //        .setMessage("车牌无效")
            //        .Build();
            //}
            //if (string.IsNullOrWhiteSpace(request.GateId))
            //{
            //    return APIReply<ParkingDetail>.newBuilder()
            //        .setResultCode(ResultCode.BadRequest)
            //        .setMessage("通道无效")
            //        .Build();
            //}
            //ParkingDetail payload = new ParkingDetail();
            //payload.EnterTime = DateTime.Now;
            //payload.ParkingTime = 0;
            //payload.CarType = 1;
            //payload.ParkingSerial = DateTime.Now.ToString("yyyyMMddHHmmss");

            //if (request.GateId.Trim() == "001")
            //{ //1次称重入口闸道
            //    payload.CarDesc = "无牌车从通道001入场";
            //    payload.EnterGate = "001";
            //    App.Current.Dispatcher.Invoke((Action)delegate ()
            //    {
            //        AWSV2.Globalspace._lprDevNo = "A";
            //        Globalspace._plateNo = request.Passport;
            //        Globalspace.ShellViewModel.GetPlateNo = request.Passport;
            //        // Globalspace.ShellViewModel.Timer_Tick(null, null);
            //        Globalspace.ShellViewModel.OpenBarrier("A");
            //    });

            //    return APIReply<ParkingDetail>.newBuilder()
            //          .setResultCode(ResultCode.Success)
            //          .setPayload(payload)
            //          .Build();
            //}
            //else if (request.GateId.Trim() == "003")
            //{ //2次称重入口闸道
            //    payload.CarDesc = "无牌车从通道003入场";
            //    payload.EnterGate = "003";
            //    App.Current.Dispatcher.Invoke((Action)delegate ()
            //    {
            //        AWSV2.Globalspace._lprDevNo = "B";
            //        Globalspace._plateNo = request.Passport;
            //        Globalspace.ShellViewModel.GetPlateNo = request.Passport;
            //        //Globalspace.ShellViewModel.Timer_Tick(null,null);
            //        Globalspace.ShellViewModel.OpenBarrier("B");
            //    });
            //    return APIReply<ParkingDetail>.newBuilder()
            //      .setResultCode(ResultCode.Success)
            //      .setPayload(payload)
            //      .Build();
            //}
            //else
            //{
            //    return APIReply<ParkingDetail>.newBuilder()
            //        .setResultCode(ResultCode.BadRequest)
            //        .setMessage("通道无效")
            //        .Build();
            //}

        }


        /// <summary>
        /// 查询停车场实时信息
        /// 1001 获取成功，业务参数将返回。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<ParkingRealtime> ParkingRealtime(ParkingRealtimeRequest request)
        {
            return base.ParkingRealtime(request);
        }

        /// <summary>
        /// 同步停车记录
        /// 1001 获取成功，业务参数将返回。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<ParkingSYNCPayload> ParkingSYNC(ParkingSYNCRequest request)
        {
            return base.ParkingSYNC(request);
        }

        /// <summary>
        /// 更新自动支付状态
        /// 1001 接口处理成功，业务参数将返回。
        /// 1002 停车记录不存在
        /// 1403 车辆已出场。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<object> ParkingAutopayUpdate(ParkingAutopayUpdateRequest request)
        {
            return base.ParkingAutopayUpdate(request);
        }

        /// <summary>
        /// 更新锁车状态
        /// 1001 接口处理成功，业务参数将返回。
        /// 1002 停车记录不存在
        /// 1403 车辆已出场。
        /// 1500 接口处理异常。
        /// </summary>
        public virtual APIReply<object> ParkingLockingUpdate(ParkingLockingUpdateRequest request)
        {
            return base.ParkingLockingUpdate(request);
        }

        /// <summary>
        /// 获取停车详情
        /// 1001 订单查询成功，业务参数将返回。
        /// 1002 未查询到停车信息。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<ParkingDetail> ParkingDetail(ParkingDetailRequest request)
        {
            ParkingPayment payment = new ParkingPayment();
            payment.PayType = PYun.API.Reply.ParkingPayment.PAY_TYPE_CASH;

            ParkingDetail detail = new ParkingDetail();
            detail.PlateColor = ColorType.Blue;
            return base.ParkingDetail(request);
        }

        /// <summary>
        /// 下发停车优惠
        /// 1001 接口处理成功，业务参数将返回。
        /// 1002 停车信息未找到。
        /// 1403 当前车辆已享受其他优惠。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<object> ParkingDiscountCreate(ParkingDiscountCreateRequest request)
        {
            return base.ParkingDiscountCreate(request);
        }

        /// <summary>
        /// 撤销停车优惠
        /// 1001 接口处理成功，业务参数将返回。
        /// 1002 派发信息未找到。
        /// 1403 优惠已使用。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<object> ParkingDiscountDestory(ParkingDiscountDestoryRequest request)
        {
            return base.ParkingDiscountDestory(request);
        }

        /// <summary>
        /// 查询月卡信息
        /// 1001 获取成功，业务参数将返回。
        /// 1002 没有查到相关贵宾记录。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<ParkingVIPPayload> ParkingVIPQuery(ParkingVIPQueryRequest request)
        {
            ParkingVIPPayload payload = new ParkingVIPPayload();
            List<ParkingVIP> Vips = new List<ParkingVIP>();
            ParkingVIP item = new ParkingVIP();
            item.Balance = 10;
            Vips.Add(item);

            payload.SetVips(Vips);

            return APIReply<ParkingVIPPayload>.newBuilder()
                .setPayload(payload)
                .setResultCode(ResultCode.Success)
                .setMessage("NOT IMPLEMENT YET")
                .Build();
        }

        /// <summary>
        /// 下发月卡续费信息
        /// 1001 续费成功，业务参数将返回。
        /// 1002 贵宾信息未找到。
        /// 1403 贵宾已被禁用，需退款。
        /// 1500 接口处理异常。
        /// </summary>
        public override APIReply<object> ParkingVIPRenewal(ParkingVIPRenewalRequest request)
        {
            return base.ParkingVIPRenewal(request);
        }

    }
}
