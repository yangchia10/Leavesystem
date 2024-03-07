<template>
  <div class="app-container common-list-page">
    <h2 style="text-align: center">密码重置</h2>
    <el-form
      :model="resetForm"
      :rules="resetFormRules"
      ref="resetForm"
      status-icon
      label-width="100px"
    >
      <el-form-item label="用户名：" prop="password">
        <el-input type="username" v-model="resetForm.username" disabled auto-complete="off"></el-input>
      </el-form-item>
      <el-form-item label="旧密码：" prop="password">
        <el-input type="password" v-model="resetForm.password" auto-complete="off"></el-input>
      </el-form-item>
      <el-form-item label="新密码：" prop="newpwd">
        <el-input type="password" v-model="resetForm.newpwd" auto-complete="off"></el-input>
      </el-form-item>
      <el-form-item label="确认密码：" prop="newpassword1">
        <el-input type="password" v-model="resetForm.newpassword1" auto-complete="off"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click.native.prevent="toAmend">确认修改</el-button>
        <el-button @click="logout">返回</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { UserModifyPassword } from '@/api/Sys/Sys_User.js'
import md5 from 'js-md5'

export default {
  data() {
    let validatePass = (rule, value, callback) => {
      if (!value) {
        callback(new Error('请输入新密码'))
      } else if (value.toString().length < 6 || value.toString().length > 18) {
        callback(new Error('密码长度为6-18位'))
      } else {
        callback()
      }
    }
    let validatePass2 = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请再次输入密码'))
      } else if (value !== this.resetForm.newpwd) {
        callback(new Error('两次输入密码不一致!'))
      } else {
        callback()
      }
    }
    return {
      resetForm: {
        //传给后台所需要的参数
        newpassword1: '',
        password: '',
        username: ''//此处只是后台需要的字段而已，如果前期有公用cookie里面有获取并且保存过，现在需要另外调用进来，具体的获取方法就看个人了
      },
      resetFormRules: {
        password: [
          { required: true, message: '请输入旧密码', trigger: 'blur' }
        ],
        newpwd: [
          { required: true, validator: validatePass, trigger: 'blur' }
        ],
        newpassword1: [
          { required: true, validator: validatePass2, trigger: 'blur' }
        ]
      }
    }
  },
  mounted() {
    this.resetForm.username = this.$route.query.userName
  },
  methods: {
    toAmend() {
      this.$refs.resetForm.validate(valid => {
        if (valid) {
          const resetData = {
            UserName: this.resetForm.username,
            oldPassword: md5(this.resetForm.password),
            newPassword: md5(this.resetForm.newpassword1)
          }
          UserModifyPassword(resetData).then(res => {
            if (res.Code === 2000) {
              this.$message.success('修改成功')
              this.logout()
            }
          })
        }
      })
    },
    //这是修改成功后重新返回登陆页的方法，看个人需要自行调整
    async logout() {
      // await this.$store.dispatch('user/logout')
      this.$router.push(`/login`)
    }
  }
}
</script>

<style lang="scss" scoped>
.el-form {
  width: 60%;
  margin: 50px auto 0;
  text-align: center;

  button {
    margin: 20px 0 0;
  }
}
</style>
