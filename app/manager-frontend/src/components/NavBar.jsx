import {
  Box,
  Button,
  Flex,
  Heading,
  HStack,
  Icon,
  Menu,
  MenuButton,
  MenuItem,
  MenuList,
  Text,
  useDisclosure,
} from "@chakra-ui/react";
import { useBackendApi } from "contexts/BackendApi";
import { useUser } from "contexts/User";
import { useTranslation } from "react-i18next";
import {
  FaChevronDown,
  FaSignInAlt,
  FaSignOutAlt,
  FaUserCircle,
  FaUserPlus,
} from "react-icons/fa";
import { Link, useNavigate } from "react-router-dom";
import { LoadingModal } from "./LoadingModal";
import Flags from "country-flag-icons/react/3x2";
import { hasFlag } from "country-flag-icons";
import { forwardRef } from "react";

const BrandLink = () => {
  const { t } = useTranslation();
  return (
    <Link to="/">
      <Heading p={2} size="lg">
        {t("buttons.brand")}
      </Heading>
    </Link>
  );
};

const NavBarButton = forwardRef(function NavBarButton({ children, ...p }, ref) {
  return (
    <Button
      ref={ref}
      height="100%"
      borderRadius={0}
      variant="ghost"
      _focus={{}}
      _hover={{ bg: "blue.500" }}
      _active={{ bg: "blue.400" }}
      {...p}
    >
      {children}
    </Button>
  );
});

const UserMenu = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { user, signOut } = useUser();
  const {
    account: { logout },
  } = useBackendApi();

  const busyModalState = useDisclosure();
  const busyModal = (
    <LoadingModal
      isOpen={busyModalState.isOpen}
      verb={t("logout.feedback.busy")}
    />
  );

  const handleLogoutClick = async () => {
    busyModalState.onOpen();

    await logout();
    signOut();
    navigate("/account/login", {
      state: {
        toast: {
          title: t("logout.feedback.success"),
          status: "success",
          duration: 2500,
          isClosable: true,
        },
      },
    });

    busyModalState.onClose();
  };

  return user ? (
    <>
      <NavBarButton leftIcon={<FaUserCircle />} as={Link} to="#">
        {user.fullName}
      </NavBarButton>
      <NavBarButton leftIcon={<FaSignOutAlt />} onClick={handleLogoutClick}>
        {t("buttons.logout")}
      </NavBarButton>
      {busyModal}
    </>
  ) : (
    <>
      <NavBarButton leftIcon={<FaSignInAlt />} as={Link} to="/account/login">
        {t("buttons.login")}
      </NavBarButton>

      <NavBarButton leftIcon={<FaUserPlus />} as={Link} to="/account/register">
        {t("buttons.register")}
      </NavBarButton>
      {busyModal}
    </>
  );
};

const FlagIcon = ({ countryIso2 }) => (
  <Icon boxSize={6} as={hasFlag(countryIso2) ? Flags[countryIso2] : Flags.US} />
);

// TODO: Hutch doesn't use this today, but it might one day
// eslint-disable-next-line
const LanguageMenu = () => {
  const { t, i18n } = useTranslation();
  const { users } = useBackendApi();
  const { user } = useUser();

  return (
    <Box height="100%">
      <Menu>
        <MenuButton
          lineHeight={0}
          as={NavBarButton}
          rightIcon={<FaChevronDown />}
        >
          <FlagIcon countryIso2={t("flagCountry")} />
        </MenuButton>
        <MenuList>
          {i18n.options.preload
            .filter((x) => x !== i18n.resolvedLanguage)
            .map((lng) => {
              return (
                <MenuItem
                  key={lng}
                  onClick={() => {
                    i18n.changeLanguage(lng);
                    user && users.setUICulture(lng);
                  }}
                >
                  <HStack>
                    <FlagIcon countryIso2={t("flagCountry", { lng })} />
                    <Text>{t("name", { lng })}</Text>
                  </HStack>
                </MenuItem>
              );
            })}
        </MenuList>
      </Menu>
    </Box>
  );
};

export const NavBar = () => {
  const { user } = useUser();

  return (
    <Flex
      pl={2}
      boxShadow="section-h"
      zIndex={1000}
      bgGradient="radial(circle 400px at top left, cyan.600, blue.900)"
      color="white"
    >
      <BrandLink />
      <HStack spacing={0} flexGrow={1} justify="end">
        {/* TODO: more links */}
        <UserMenu />
      </HStack>
    </Flex>
  );
};
