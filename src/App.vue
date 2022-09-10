<script setup lang="ts">
import { darkTheme } from 'naive-ui';
import { RouterLink, RouterView } from 'vue-router'
import HelloWorld from './components/HelloWorld.vue'
import type { MenuOption } from 'naive-ui'
import { NIcon } from 'naive-ui'
import { Home as HomeIcon, Book as BookIcon } from '@vicons/ionicons5'
import type { Component } from 'vue';

const siderHide = ref(false)
const renderIcon = (icon: Component) => {
  return () => h(NIcon, null, { default: () => h(icon) })
}
const menuOptions: MenuOption[] = [
  {
    label: () =>
      h(
        RouterLink,
        {
          to: {
            name: 'home'
          },
        },
        {
          default: () => '主页',
        }
      ),
    key: 'home',
    icon: renderIcon(HomeIcon),
  },
] 
</script>

<template>
  <n-config-provider :theme="darkTheme">
    <n-global-style />

    <n-layout id="main-layout">
      <n-layout-header id="header-layout" content-style="padding: 24px;">
      </n-layout-header>
      <n-layout has-sider id="sub-layout">
        <n-layout-sider id="sider-layout" collapse-mode="width" :collapsed-width="64" :width="240"
          :collapsed="siderHide" show-trigger @collapse="siderHide=true" @expand="siderHide=false">
          <n-menu :collapse="siderHide" :collapsed-width="64" :collapsed-icon-size="22" :options="menuOptions">

          </n-menu>
        </n-layout-sider>
        <n-layout-content id="content-layout" content-style="padding: 24px;">
          <n-scrollbar x-scrollable>
            <RouterView></RouterView>
          </n-scrollbar>
        </n-layout-content>
      </n-layout>
    </n-layout>

  </n-config-provider>
</template>


<style scoped>
#main-layout {
  height: 100vh;
  width: 100vw;
  position: absolute;
  left: 0;
  top: 0;
}


#header-layout {
  height: 80px;
  width: 100%;
}

#sider-layout {
  height: 100%;
}

#sub-layout {
  height: calc(100% - 80px);
  width: 100%;
}
</style>
