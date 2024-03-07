<template>
  <el-form ref="dataForm" :model="user.userinfo" :rules="rules" label-width="120px">
    <el-form-item :label="$t('ui.Sys.Sys_User.LoginAccount')">
      <el-input v-model="user.userinfo.LoginAccount" :disabled="true"/>
    </el-form-item>
    <el-form-item :label="$t('ui.Sys.Sys_User.UserName')" prop="UserName">
      <el-input v-model="user.userinfo.UserName"/>
    </el-form-item>
    <el-form-item :label="$t('ui.Sys.Sys_User.FrgnName')">
      <el-input v-model="user.userinfo.FrgnName" autocomplete="off"/>
    </el-form-item>

    <el-form-item :label="$t('ui.Sys.Sys_User.Gender')">
      <el-select v-model="user.userinfo.Gender" class="filter-item">
        <el-option
          v-for="item in genderOptions"
          :key="item.value"
          :label="item.label"
          :value="item.value"
        />
      </el-select>
    </el-form-item>
    <el-form-item :label="$t('ui.Sys.Sys_User.OrganizationID')">
      <treeselect
        v-model="user.userinfo.OrganizationID"
        :multiple="false"
        :default-expand-level="1"
        :options="treeOptions"
        :placeholder="$t('Common.select')+$t('ui.Sys.Sys_User.OrganizationID')"
      />
    </el-form-item>
    <el-form-item :label="$t('ui.Sys.Sys_User.Birthday')">
      <el-date-picker v-model="user.userinfo.Birthday" type="date"/>
    </el-form-item>
    <el-form-item :label="$t('ui.Sys.Sys_User.Email')" prop="Email">
      <el-input v-model="user.userinfo.Email" autocomplete="off"/>
    </el-form-item>
    <el-form-item :label="$t('ui.Sys.Sys_User.Mobile')" prop="Mobile">
      <el-input v-model="user.userinfo.Mobile" autocomplete="off"/>
    </el-form-item>
    <el-form-item :label="$t('ui.Sys.Sys_User.Telphone')">
      <el-input v-model="user.userinfo.Telphone" autocomplete="off"/>
    </el-form-item>

    <el-form-item label="ERP账号">
      <el-input v-model="user.userinfo.ERPCode" :disabled="true"/>
    </el-form-item>
    <el-form-item
      label="ERP密码"
    >
      <el-input v-model="user.userinfo.ERPPwd" show-password/>
    </el-form-item>
    <el-form-item :label="$t('Common.Remark')">
      <el-input v-model="user.userinfo.Remark" :autosize="{ minRows: 3, maxRows: 5}" type="textarea"/>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="updateData">{{ $t('Common.confirm') }}</el-button>
    </el-form-item>
  </el-form>
</template>

<script>

import Treeselect from '@riophae/vue-treeselect'
import '@riophae/vue-treeselect/dist/vue-treeselect.css'
import { convertToKeyValue, listToTree, convertToSelectTree } from '@/utils'
import waves from '@/directive/waves' // waves directive 特效
import { update } from '@/api/Sys/Sys_User'
import { fetchList as fetchOrganizationList } from '@/api/Sys/Sys_Organization'
import { validateEMail, validatePhone } from '@/utils/form-validator'  // 表单自定义校验

export default {
  props: {
    user: {
      type: Object,
      default: () => {
        return {
          name: '',
          userinfo: {
            UserID: null,
            OrganizationID: null,
            OrganizationDesc: '',
            UserName: '',
            FrgnName: '',
            LoginAccount: '',
            LoginPassword: '',
            Gender: '',
            Birthday: new Date(),
            Email: '',
            Telphone: '',
            Mobile: '',
            UserRole: 2, // 默认业务用户
            IsEnable: true,
            Remark: '',
            CUser: '',
            CDate: new Date(),
            MUser: '',
            MDate: new Date(),
            ERPPwd: '',
            ERPCode: ''
          },
          avatar: '',
          roles: ''
        }
      }
    }
  },
  components: { Treeselect },
  data() {
    return {
      listLoading: true,
      listQuery: {
        keyword: '', // 用户姓名 | 登录账号
        PageNumber: 1,
        PageSize: 10
      },
      genderOptions: [
        { value: 1, label: this.$t('Dictionary.GenderMap.One') },  // 男
        { value: 2, label: this.$t('Dictionary.GenderMap.Two') } // 女
      ],
      // define the default value
      treeSelectedValue: null,
      // define options
      treeOptions: [],
      organizationList: [],
      rules: {
        // 表单校验逻辑
        UserName: [
          {
            required: true,
            message: i18n.t('Common.ValidatorMessage.MustInput'),
            trigger: 'blur'
          }
        ],
        Mobile: [
          {
            validator: validatePhone,
            message: this.$t('ui.Sys.Sys_User.FormValidator.ValidateMobile'),
            trigger: 'blur'
          }
        ],
        Email: [
          {
            validator: validateEMail,
            message: this.$t('ui.Sys.Sys_User.FormValidator.ValidateEmail'),
            trigger: 'blur'
          }
        ]
      }
    }
  },
  created() {
    this.initOrganizationTree()  // 组织机构Formatter
  },
  methods: {
    initOrganizationTree() {
      fetchOrganizationList(this.listQuery).then(response => {
        this.treeOptions = convertToSelectTree(
          response.Data.items,
          'OrganizationID',
          'FathOrganizationID',
          'OrganizationDesc'
        )
      })
    },
    updateData() {
      // console.log(this.user.userinfo)
      this.$refs['dataForm'].validate(valid => {
        if (valid) {
          update(this.user.userinfo)
            .then(() => {
              this.$notify({
                title: this.$t('Common.success'),
                message: this.$t('Common.operationSuccess'),
                type: 'success',
                duration: 2000
              })
              this.$store
                .dispatch('user/changeUserInfo', this.user.userinfo)
            })
        }

      })

    }
  }
}
</script>
