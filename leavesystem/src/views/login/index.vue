<template>
  <div class="login-container">
    <el-form
      ref="loginForm"
      :model="loginForm"
      :rules="loginRules"
      class="login-form"
      autocomplete="on"
      label-position="left"
    >
      <div class="title-container">
        <!-- <div class="Logo">
          <img src="../../assets/Logo2.png"/>
        </div> -->
        <h3 class="title">{{ $t('login.title') }}</h3>
        <!-- <lang-select class="set-language" /> -->
      </div>

      <el-form-item prop="username">
        <span class="svg-container">
          <svg-icon icon-class="user" />
        </span>
        <el-input
          ref="username"
          v-model="loginForm.username"
          :placeholder="$t('login.username')"
          name="username"
          type="text"
          tabindex="1"
          autocomplete="on"
          class="svg-input"
        />
      </el-form-item>

      <el-tooltip
        v-model="capsTooltip"
        :content="$t('Common.CheckCapslock')"
        placement="right"
        manual
      >
        <el-form-item prop="password">
          <span class="svg-container">
            <svg-icon icon-class="password" />
          </span>
          <el-input
            :key="passwordType"
            ref="password"
            v-model="loginForm.password"
            :type="passwordType"
            :placeholder="$t('login.password')"
            name="password"
            tabindex="2"
            autocomplete="on"
            class="svg-input"

            @keyup.native="checkCapslock"
            @blur="capsTooltip = false"
            @keyup.enter.native="handleLogin"
          />
          <span class="show-pwd" @click="showPwd">
            <svg-icon :icon-class="passwordType === 'password' ? 'eye' : 'eye-open'" />
          </span>
        </el-form-item>
      </el-tooltip>

      <el-button
        :loading="loading"
        type="primary"
        style="width:100%;margin-bottom:30px;"
        @click.native.prevent="handleLogin"
      >{{ $t('login.logIn') }}
      </el-button>
    </el-form>
  </div>
</template>

<script>
// import { validUsername } from '@/utils/validate'
// import LangSelect from '@/components/LangSelect'
import md5 from 'js-md5'
// import SocialSign from './components/SocialSignin'

export default {
  name: 'Login',
  components: {
    // LangSelect
    // , SocialSign
  },
  data() {
    // const validateUsername = (rule, value, callback) => {
    //   if (!validUsername(value)) {
    //     callback(new Error('Please enter the correct user name'))
    //   } else {
    //     callback()
    //   }
    // }
    // const validatePassword = (rule, value, callback) => {
    //   if (value.length < 6) {
    //     callback(new Error('The password can not be less than 6 digits'))
    //   } else {
    //     callback()
    //   }
    // }
    return {
      loginForm: {
        username: '',
        password: ''
      },
      loginRules: {
        username: [
          {
            required: true,
            message: this.$t('Common.ValidatorMessage.MustInput'),
            trigger: 'change'
          },
          {
            required: true,
            min: 3,
            max: 15,
            message: this.$t('Common.ValidatorMessage.LengthRange', {
              Range: '[3-15]'
            }),
            trigger: 'change'
          }
        ],
        password: [
          {
            required: true,
            min: 3,
            max: 15,
            message: this.$t('Common.ValidatorMessage.LengthRange', {
              Range: '[3-15]'
            }),
            trigger: 'change'
          }
        ]
      },
      passwordType: 'password',
      capsTooltip: false,
      loading: false,
      showDialog: false,
      redirect: undefined,
      otherQuery: {}
    }
  },
  watch: {
    $route: {
      handler: function(route) {
        // console.log('$route', route)
        const query = route.query
        if (query) {
          this.redirect = query.redirect
          this.otherQuery = this.getOtherQuery(query)
        }
      },
      immediate: true
    }
  },
  created() {
    // window.addEventListener('storage', this.afterQRScan)
  },
  mounted() {
    if (this.loginForm.username === '') {
      this.$refs.username.focus()
    } else if (this.loginForm.password === '') {
      this.$refs.password.focus()
    }
  },
  destroyed() {
    // window.removeEventListener('storage', this.afterQRScan)
  },
  methods: {
    checkCapslock({ shiftKey, key } = {}) {
      if (key && key.length === 1) {
        if (
          (shiftKey && key >= 'a' && key <= 'z') ||
          (!shiftKey && key >= 'A' && key <= 'Z')
        ) {
          this.capsTooltip = true
        } else {
          this.capsTooltip = false
        }
      }
      if (key === 'CapsLock' && this.capsTooltip === true) {
        this.capsTooltip = false
      }
    },
    showPwd() {
      if (this.passwordType === 'password') {
        this.passwordType = ''
      } else {
        this.passwordType = 'password'
      }
      this.$nextTick(() => {
        this.$refs.password.focus()
      })
    },
    handleLogin() {
      // 登陆按钮处理事件
      this.$refs.loginForm.validate(valid => {
        if (valid) {
          // 前端校验通过
          this.loading = true
          // console.log('handlelogin',md5('123456'))
          // this.loginForm.password = md5(this.loginForm.password)
          const loginData = {
            username: this.loginForm.username,
            password: md5(this.loginForm.password)
          }
          this.$store
            .dispatch('user/login', loginData)
            .then((data) => {
              if (data.Message === 'api.Sys.Sys_User.NeedUpdatePassword') {
                this.$alert(this.i18n.t(data.Message), {
                  confirmButtonText: this.i18n.t('Common.affirm'),
                  callback: action => {
                    // 校验通过，跳转到布局首页
                    // console.log('login.otherQuery', this.otherQuery)
                    // console.log('login.redirect', this.redirect)
                    // this.$router.push({
                    //   path: this.redirect || '/',
                    //   query: this.otherQuery
                    // })
                    this.$router.push({
                      path: '/resetPassword',
                      query: { userName: this.loginForm.username }
                    })
                    this.loading = false
                  }
                })
              } else {
                // 校验通过，跳转到布局首页
                // console('login.otherQuery',this.otherQuery)
                // console('login.redirect',this.redirect)
                this.$router.push({
                  path: this.redirect || '/',
                  query: this.otherQuery
                })
                this.loading = false
              }
            })
            // eslint-disable-next-line handle-callback-err
            .catch((error) => {
              this.loading = false
            })
        } else {
          console.log('error submit!!')
          return false
        }
      })
    },
    getOtherQuery(query) {
      return Object.keys(query).reduce((acc, cur) => {
        if (cur !== 'redirect') {
          acc[cur] = query[cur]
        }
        return acc
      }, {})
    }
  }
}
</script>

<style lang="scss">
/* 修复input 背景不协调 和光标变色 */
/* Detail see https://github.com/PanJiaChen/vue-element-admin/pull/927 */

$bg: #DAE1E4;
$light_gray: #3F7E9A;
$cursor: #3F7E9A;

@supports (-webkit-mask: none) and (not (cater-color: $cursor)) {
  .login-container .el-input input {
    color: $cursor;
  }
}

/* reset element-ui css */
.login-container {
  .el-input {
    display: inline-block;
    height: 47px;
    width: 85%;

    input {
      background: transparent;
      border: 0px;
      -webkit-appearance: none;
      border-radius: 0px;
      padding: 12px 5px 12px 15px;
      color: $light_gray;
      height: 47px;
      caret-color: $cursor;
      font-weight: bold;

      &:-webkit-autofill {
        box-shadow: 0 0 0px 1000px $bg inset !important;
        -webkit-text-fill-color: $cursor !important;

      }
    }
  }

  .el-form-item {
    border: 1px solid rgba(255, 255, 255, 0.1);
    background: rgba(161, 87, 87, 0.1);
    border-radius: 5px;
    color: #454545;
  }
}
</style>

<style lang="scss" scoped>
$bg: #62d5d5;
$dark_gray: #3F7E9A;
$light_gray: #fff;

.login-container {
  min-height: 100%;
  width: 100%;
  /*background-color: $bg;*/
  overflow: hidden;
  background: url("../../assets/LoginBg.jpg") no-repeat center top;
  background-size: 100% 100%;

  .login-form {
    position: relative;
    width: 520px;
    max-width: 100%;
    padding: 0 35px 0;
    margin: 15% auto 35px;
    overflow: hidden;
    /*background: rgba(255, 255, 255, 0.5);*/
    /*border-radius: 20px;*/

  }

  .tips {
    font-size: 14px;
    color: #fff;
    margin-bottom: 10px;

    span {
      &:first-of-type {
        margin-right: 16px;
      }
    }
  }

  .svg-container {
    padding: 6px 5px 6px 15px;
    color: $dark_gray;
    vertical-align: middle;
    width: 30px;
    display: inline-block;
  }
    .svg-input {
   background-color: #fff;
   font-size: medium;
  }

  .title-container {
    position: relative;
    display: flex;// flex
    align-items: center;
    justify-content: space-between;

    .title {
      font-size: 26px;
      color: $light_gray;
      padding: 0 auto 100px auto;
      text-align: center;
      font-weight: bold;
      //text-shadow: 1px 1px 0px #FFFFFF, -1px -1px 0px #FFFFFF, 2px 2px 0px #FFFFFF, -2px -2px 0px #FFFFFF, 3px 3px 0px #FFFFFF, -3px -3px 0px #FFFFFF;
      text-shadow: 0 0 10px rgb(187, 140, 140);
    }

    .set-language {
      color: rgb(187, 84, 84);
      position: absolute;
      top: 3px;
      font-size: 18px;
      right: 0px;
      cursor: pointer;
    }

    .Logo {
      width: 34%;
      margin-right: 5%;

      img {
        width: 100%;
        height: auto;
      }
    }
  }

  .show-pwd {
    position: absolute;
    right: 10px;
    top: 7px;
    font-size: 16px;
    color: $dark_gray;
    cursor: pointer;
    user-select: none;
  }

  .thirdparty-button {
    position: absolute;
    right: 0;
    bottom: 6px;
  }

  @media only screen and (max-width: 470px) {
    // .thirdparty-button {
      // display: none;
    // }
  }
}
</style>
