<template>
  <div class="app-container">
    <div v-if="user">
      <el-row :gutter="20">
        <el-col :span="6" :xs="24">
          <user-card :user="user" />

          <el-button class="filter-item" style="width: 100%" type="primary" icon="el-icon-printer" @click="loginCode">
            打印登錄碼
          </el-button>

        </el-col>

        <el-col :span="18" :xs="24">
          <el-card>
            <el-tabs v-model="activeTab">
              <el-tab-pane label="帳號訊息" name="account">
                <account :user="user" />
              </el-tab-pane>
              <el-tab-pane label="修改密碼" name="updatepassword">
                <update-password :user="user" />
              </el-tab-pane>
              <!-- <el-tab-pane label="個人活動" name="activity">
                <activity />
              </el-tab-pane>
              <el-tab-pane label="個人時光" name="timeline">
                <timeline />
              </el-tab-pane> -->
            </el-tabs>
          </el-card>
        </el-col>
      </el-row>
    </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'
import UserCard from './components/UserCard'
import Account from './components/Account'
import UpdatePassword from './components/UpdatePassword'
// import md5 from 'js-md5'
// import Activity from './components/Activity'
// import Timeline from './components/Timeline'
import { Print } from '@/api/Sys/Sys_User'

export default {
  name: 'Profile',
  components: {
    UserCard, Account, UpdatePassword
    // ,Activity, Timeline
  },
  data() {
    return {
      user: {},
      activeTab: 'account'
    }
  },
  computed: {
    ...mapGetters([
      'name',
      'avatar',
      'roles',
      'userinfo'
    ])
  },
  created() {
    if (this.$route.query.activeTab !== undefined) {
      this.activeTab = this.$route.query.activeTab
    }

    this.getData()
    // const permissionbuttons = this.$store.state.user.permissionbuttons.length > 0 ? this.$store.state.user.permissionbuttons : JSON.parse(sessionStorage.getItem('permissionbuttons'))
    // const userff = this.$store.state.user.userinfo?this.$store.state.user.userinfo:JSON.parse(sessionStorage.getItem('userinfo'))
    // console.log('userff',userff)
    // console.log('this.user',this.user)
    // console.log('this.$store.state.sessionstorage',sessionStorage.getItem('permissionbuttons'))
  },
  methods: {
    getData() {
      this.user = {
        name: this.name,
        userinfo: Object.assign(this.userinfo, { ERPPwd: '' }),
        avatar: this.avatar,
        roles: this.roles.join(' | ')
      }
    },
    loginCode() {
      Print([this.user.userinfo.UserID]).then(response => {
        window.open(response.Data.PrintedPDF)
      })
    }

  }
}
</script>
