<template>
    <div>
        <img id="logo" src="images/cu-online.png" />
        <div id="content" v-show="!isLoading">

            <h2>Embed Tool</h2>
            <a href="webUrl" target="_blank">https://ucdenver.instructure.com</a>
            <br />
            
            <hr />

            <div class="row">
                <div v-show="generateError" class="card card-body bg-warning mb-2">
                    Invalid embed URL
                </div>

                <form class="form-inline" style="width: 100%">
                    <div class="form-group">
                        <label>Embed Url:</label>
                        <input class="form-control" v-model="embedUrl" />
                    </div>
                    <div class="form-group">
                        <label>Account Navigation:</label>
                        <input class="form-check" type="checkbox" v-model="accountNav" />
                    </div>

                    <div class="form-group">
                        <label>Course Navigation:</label>
                        <input class="form-check" type="checkbox" v-model="courseNav" />
                    </div>

                    <div class="form-group">
                        <label>User Navigation:</label>
                        <input class="form-check" type="checkbox" v-model="userNav" />
                    </div>

                    <input type="button" class="btn btn-sm btn-cu" value="Generate" @click="generateConfig()" />
                </form>
            </div>

            <hr />

            <div>
                <p>
                    This page will help you generate the XML configuration needed to add an embedded page in Canvas. 
                    Enter the URL of the page you wish to embed and where you would like it to appear in Canvas. 
                    Note that the specified will not show up in Canvas unless it has X-Frame-Options headers configured to allow embedding.
                </p>
            </div>

            <hr />

            <div>
                <p>
                    After clicking "Generate", you will be redirected to a page with configuration data. Follow these instructions to add your generated config to a course or account.
                </p>

                <ol>
                    <li>
                        Navigate to the course or account where you wish to add embedded page. Click "Settings" on the left-hand sidebar navigation, go to the "Apps" tab, and click the "+ App" button.

                        <img src="images/add_app_example.png" class="example" />
                    </li>

                    <li>
                        In the App configuration popup, populate the following fields:

                        <ul>
                            <li>Configuration Type: "By URL"</li>
                            <li>Name: An identifier of your choosing.</li>
                            <li>Config URL: The URL of your generated configuration.</li>
                            <li>
                                Consumer Key & Shared Secret: These can be obtained on the Developer Keys page. 
                                Look for a key with a "URI" matching the domain of this page, and use the "ID" and "Key" as the app's "Consumer Key" and "Shared Secret", repectively.
                            
                                <img src="images/dev_keys_example.png" class="example" />
                            </li>
                            <li>
                                Click "Submit" and refresh the page. The item should appear in the specified navigation area with the name you configured.
                            </li>
                        </ul>

                    </li>
                </ol>
            </div>
            
        </div>
    </div>
</template>
<script>
    import Vue from "vue";
    import axios from "axios";
    import _ from "lodash";
    import filedownload from "js-file-download";

    export default Vue.extend({
        props: {
            baseUrl: String
        },
        data() {
            return {
                isLoading: false,
                isSaving: false,
                error: "",
                embedUrl: "",
                accountNav: false,
                courseNav: false,
                userNav: false,
                generateError: false
            }
        },
        mounted() {
            this.isLoading = true;
            this.isLoading = false;
        },
        methods: {
            generateConfig() {
                this.generateError = false;

                if (this.embedUrl.length <= 0 || !this.validURL(this.embedUrl)) {
                    this.generateError = true;
                    return;
                }

                let url = this.baseUrl + "Home/Config?url=" + this.embedUrl + "&accountNavigation=" + this.accountNav 
                    + "&courseNavigation=" + this.courseNav + "&userNavigation=" + this.userNav;

                window.location = url;
            },
            validURL(str) {
                var pattern = new RegExp('^(https?:\\/\\/)?'+
                '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|'+
                '((\\d{1,3}\\.){3}\\d{1,3}))'+
                '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*'+
                '(\\?[;&a-z\\d%_.~+=-]*)?'+
                '(\\#[-a-z\\d_]*)?$','i');
                return !!pattern.test(str);
            }
        }
    });
</script>