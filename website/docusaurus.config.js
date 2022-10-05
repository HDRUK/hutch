// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion

const lightCodeTheme = require("prism-react-renderer/themes/github");
const darkCodeTheme = require("prism-react-renderer/themes/dracula");

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: "Hutch",
  tagline: "Dinosaurs are cool",
  url: "https://your-docusaurus-test-site.com",
  baseUrl: "/",
  trailingSlash: false,
  onBrokenLinks: "throw",
  onBrokenMarkdownLinks: "throw",
  favicon: "favicon.svg",

  // GitHub pages deployment config.
  // If you aren't using GitHub pages, you don't need these.
  organizationName: "hdruk", // Usually your GitHub org/user name.
  projectName: "hutch", // Usually your repo name.

  // Even if you don't use internalization, you can use this field to set useful
  // metadata like html lang. For example, if your site is Chinese, you may want
  // to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: "en",
    locales: ["en"],
  },

  presets: [
    [
      "classic",
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: require.resolve("./sidebars.js"),
          // Please change this to your repo.
          // Remove this to remove the "edit this page" links.
          editUrl: "https://github.com/hdruk/hutch/tree/main/website/",
        },
        blog: false,
        theme: {
          customCss: require.resolve("./src/css/custom.css"),
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      announcementBar: {
        content: "🚨🚧 This documentation site is a work in progress. 🚧🚨",
        backgroundColor: "#ffdb80",
        isCloseable: false,
      },
      navbar: {
        logo: {
          src: "img/hutch_logo-mono-black.svg",
          srcDark: "img/hutch_logo-mono-white.svg",
          alt: "Hutch logo",
        },

        items: [
          {
            type: "doc",
            docId: "users/index",
            position: "left",
            label: "User Guide",
          },
          {
            type: "doc",
            docId: "devs/index",
            position: "right",
            label: "For Developers",
          },
          {
            href: "https://github.com/hdruk/hutch",
            position: "right",
            className: "header-github-link",
            "aria-label": "GitHub repository",
          },
        ],
      },
      footer: {
        style: "dark",
        links: [
          {
            title: "Docs",
            items: [
              {
                label: "User Guide",
                to: "/docs/users",
              },
              {
                label: "For Developers",
                to: "/docs/devs",
              },
            ],
          },
          {
            title: "Links",
            items: [
              {
                label: "University of Nottingham",
                href: "https://nottingham.ac.uk",
              },
              {
                label: "UoN Digital Research Service",
                href: "https://linktr.ee/uondrs",
              },
              {
                label: "Health Data Research UK",
                href: "https://hdruk.ac.uk",
              },
            ],
          },
          {
            title: "Community",
            items: [
              {
                label: "DRS Twitter",
                href: "https://twitter.com/uondrs",
              },
            ],
          },
          {
            title: "More",
            items: [
              {
                label: "GitHub",
                href: "https://github.com/hdruk/hutch",
              },
            ],
          },
        ],
        logo: {
          alt: "University of Nottingham Logo",
          src: "/img/uon_white_text_web.png",
          href: "https://nottingham.ac.uk",
        },
        copyright: `Copyright © ${new Date().getFullYear()} University of Nottingham. Built with Docusaurus.`,
      },
      prism: {
        theme: lightCodeTheme,
        darkTheme: darkCodeTheme,
      },
    }),
};

module.exports = config;
