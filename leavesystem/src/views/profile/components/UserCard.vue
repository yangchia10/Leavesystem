<template>
  <el-card style="margin-bottom:20px;">
    <div slot="header" class="clearfix">
      <span>個人信息</span>
    </div>

    <div class="user-profile">
      <div class="box-center">
        <pan-thumb :image="user.avatar" :height="'100px'" :width="'100px'" :hoverable="false">
          <div style="color:#00CC66">您好:</div>
          {{ user.userinfo.UserName }}
        </pan-thumb>
      </div>
      <div class="box-center">
        <div class="user-name text-center">{{ user.userinfo.UserName }}</div>
        <div class="user-role text-center text-muted">賬號：{{ user.userinfo.LoginAccount }}</div>
      </div>
    </div>

    <div class="user-bio">
      <div class="user-education user-bio-section">
        <div class="user-bio-section-header"><svg-icon icon-class="education" /><span>個人簡介</span></div>
        <div class="user-bio-section-body">
          <div class="text-muted">
            {{ user.userinfo.Remark | introductionFilter }}
          </div>
        </div>
      </div>

    </div>
  </el-card>
</template>

<script>
import PanThumb from '@/components/PanThumb'

export default {
  components: { PanThumb },
  filters: {
    introductionFilter: function(value) {
      return value || '這傢伙很懶，什麼也沒留下...'
    }
  },
  props: {
    user: {
      type: Object,
      default: () => {
        return {
          name: '',
          userinfo: {},
          avatar: '',
          roles: ''
        }
      }
    }
  }
}
</script>

<style lang="scss" scoped>
 .box-center {
   margin: 0 auto;
   display: table;
 }

 .text-muted {
   color: #777;
 }

 .user-profile {
   .user-name {
     font-weight: bold;
   }

   .box-center {
     padding-top: 10px;
   }

   .user-role {
     padding-top: 10px;
     font-weight: 400;
     font-size: 14px;
   }

   .box-social {
     padding-top: 30px;

     .el-table {
       border-top: 1px solid #dfe6ec;
     }
   }

   .user-follow {
     padding-top: 20px;
   }
 }

 .user-bio {
   margin-top: 20px;
   color: #606266;

   span {
     padding-left: 4px;
   }

   .user-bio-section {
     font-size: 14px;
     padding: 15px 0;

     .user-bio-section-header {
       border-bottom: 1px solid #dfe6ec;
       padding-bottom: 10px;
       margin-bottom: 10px;
       font-weight: bold;
     }
   }
 }
</style>
