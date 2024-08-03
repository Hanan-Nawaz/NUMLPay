package com.example.numlpay.api;

import com.google.gson.JsonArray;

import model.AccountBook;
import model.BusRoute;
import model.ChallanVerification;
import model.EligibleFees;
import model.EmailModel;
import model.InstallmentManagement;
import model.ResponseMessage;
import model.SessionView;
import model.UnPaidFeeView;
import model.UnpaidFees;
import model.Users;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.Path;
import retrofit2.http.Query;

import java.util.List;

public interface ApiService {

    @POST("Users/Login")
    Call<Users> loginUser(@Body Users user);

    @GET("GenerateChallan/{id}")
    Call<List<UnPaidFeeView>> getUnPaidFees(@Path("id") String userId);

    @GET("AccountBook/{id}")
    Call<List<UnPaidFeeView>> getAccountBook(@Path("id") String userId);

    @POST("Users/GetEligibleFees")
    Call<EligibleFees> getEligibleFees(@Body EligibleFees eFees);

    @GET("InstallmentManagement/getActive/{fee_for}")
    Call<List<InstallmentManagement>> getActiveInstallments(@Path("fee_for") int feeFor);

    @GET("Session/GetforEligibleDdl/{sessionId}")
    Call<List<SessionView>> getSessionForDropdown(@Path("sessionId") int sessionId);

    @GET("BusRoute/GetActiveForDepartment/{id}")
    Call<List<BusRoute>> getActiveBusRoutes(@Path("id") int departmentId);

    @POST("GenerateChallan/{sessionId}/{feeFor}/{mode}/{degreeId}/{admissionSession}/{feePlan}/{route}")
    Call<Void> postChallan(
            @Path("sessionId") int sessionId,
            @Path("feeFor") int feeFor,
            @Path("mode") int mode,
            @Path("degreeId") int degreeId,
            @Path("admissionSession") int admissionSession,
            @Path("feePlan") int feePlan,
            @Path("route") int route,
            @Body UnpaidFees unpaidFee
    );

    @POST("GenerateChallan/{sessionId}/{feeFor}/{mode}/{degreeId}/{admissionSession}/{feePlan}/{deptId}")
    Call<Void> generateChallan(
            @Path("sessionId") int sessionId,
            @Path("feeFor") int feeFor,
            @Path("mode") int mode,
            @Path("degreeId") int degreeId,
            @Path("admissionSession") int admissionSession,
            @Path("feePlan") int feePlan,
            @Path("deptId") int deptId,
            @Body UnpaidFees unpaidFee
    );

    @GET("MiscellaneousFees/Active")
    Call<JsonArray> getMiscellaneousFees();

    @POST("GenerateChallan")
    Call<Void> generateChallan(@Body UnpaidFees unpaidFee);

    @POST("AccountBook")
    Call<Void> postAccountBook(@Body AccountBook accountBook);

    @POST("ChallanVerification")
    Call<Void> postChallanVerification(@Body ChallanVerification challanVerification);

    @POST("Users/SendEamil")
    Call<Void> sendEmail(@Body EmailModel emailModel);
}


